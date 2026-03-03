namespace Loupedeck.HomeAssistantByBatuPlugin
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal class HomeAssistantClient : IDisposable
    {
        private readonly String _url;
        private readonly String _token;
        private ClientWebSocket _ws;
        private CancellationTokenSource _cts;
        private Int32 _messageId;
        private Boolean _disposed;
        private Task _receiveTask;
        private Task _reconnectTask;

        private readonly ConcurrentDictionary<String, HaEntity> _states = new();
        private readonly ConcurrentDictionary<Int32, TaskCompletionSource<JsonElement>> _pending = new();
        private readonly SemaphoreSlim _sendLock = new(1, 1);

        private const Int32 ReconnectDelayMs = 5000;
        private const Int32 ReceiveBufferSize = 16384;

        public event EventHandler<HaStateChangedEventArgs> StateChanged;
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        /// Fires after authentication succeeds AND all states are fetched.
        public event EventHandler StatesLoaded;

        public Boolean IsConnected => _ws?.State == WebSocketState.Open;

        public HomeAssistantClient(String url, String token)
        {
            _url = url;
            _token = token;
        }

        public IReadOnlyDictionary<String, HaEntity> States => _states;

        public HaEntity GetEntity(String entityId)
        {
            _states.TryGetValue(entityId, out var entity);
            return entity;
        }

        public List<HaEntity> GetEntitiesByDomain(String domain)
        {
            return _states.Values
                .Where(e => e.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.FriendlyName)
                .ToList();
        }

        public async void ConnectAsync()
        {
            try
            {
                await this.ConnectInternalAsync();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Initial connection failed, will retry");
                this.ScheduleReconnect();
            }
        }

        private async Task ConnectInternalAsync()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _ws?.Dispose();
            _ws = new ClientWebSocket();

            PluginLog.Info($"Connecting to {_url}");
            await _ws.ConnectAsync(new Uri(_url), _cts.Token);

            _receiveTask = Task.Run(() => this.ReceiveLoopAsync(_cts.Token));
        }

        private async Task ReceiveLoopAsync(CancellationToken ct)
        {
            var buffer = new byte[ReceiveBufferSize];
            var messageBuffer = new List<byte>();

            try
            {
                while (!ct.IsCancellationRequested && _ws.State == WebSocketState.Open)
                {
                    messageBuffer.Clear();
                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            PluginLog.Info("WebSocket close received");
                            this.Disconnected?.Invoke(this, EventArgs.Empty);
                            this.ScheduleReconnect();
                            return;
                        }
                        messageBuffer.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                    }
                    while (!result.EndOfMessage);

                    var json = Encoding.UTF8.GetString(messageBuffer.ToArray());
                    this.ProcessMessage(json);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebSocketException ex)
            {
                PluginLog.Error(ex, "WebSocket error");
                this.Disconnected?.Invoke(this, EventArgs.Empty);
                this.ScheduleReconnect();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Receive loop error");
                this.Disconnected?.Invoke(this, EventArgs.Empty);
                this.ScheduleReconnect();
            }
        }

        private void ProcessMessage(String json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("type", out var typeProp))
                {
                    return;
                }

                var type = typeProp.GetString();

                switch (type)
                {
                    case "auth_required":
                        this.SendAuthAsync();
                        break;

                    case "auth_ok":
                        PluginLog.Info("Authentication successful");
                        this.OnAuthenticated();
                        break;

                    case "auth_invalid":
                        PluginLog.Error("Authentication failed - check your token");
                        break;

                    case "result":
                        this.HandleResult(root);
                        break;

                    case "event":
                        this.HandleEvent(root);
                        break;
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Error processing message");
            }
        }

        private async void SendAuthAsync()
        {
            var authMsg = JsonSerializer.Serialize(new { type = "auth", access_token = _token });
            await this.SendRawAsync(authMsg);
        }

        private async void OnAuthenticated()
        {
            this.Connected?.Invoke(this, EventArgs.Empty);

            await this.FetchStatesAsync();
            await this.SubscribeEventsAsync();

            PluginLog.Info($"Ready! {_states.Count} entities loaded.");
            this.StatesLoaded?.Invoke(this, EventArgs.Empty);
        }

        private async Task FetchStatesAsync()
        {
            try
            {
                var result = await this.SendCommandAsync("get_states");
                if (result.ValueKind == JsonValueKind.Array)
                {
                    _states.Clear();
                    foreach (var item in result.EnumerateArray())
                    {
                        var entity = JsonSerializer.Deserialize<HaEntity>(item.GetRawText());
                        if (entity?.EntityId != null)
                        {
                            _states[entity.EntityId] = entity;
                        }
                    }
                    PluginLog.Info($"Loaded {_states.Count} entities");
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed to fetch states");
            }
        }

        private async Task SubscribeEventsAsync()
        {
            try
            {
                await this.SendCommandAsync("subscribe_events", new { event_type = "state_changed" });
                PluginLog.Info("Subscribed to state_changed events");
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed to subscribe events");
            }
        }

        private void HandleResult(JsonElement root)
        {
            if (root.TryGetProperty("id", out var idProp))
            {
                var id = idProp.GetInt32();
                if (_pending.TryRemove(id, out var tcs))
                {
                    if (root.TryGetProperty("success", out var success) && success.GetBoolean())
                    {
                        if (root.TryGetProperty("result", out var resultProp))
                        {
                            tcs.SetResult(resultProp.Clone());
                        }
                        else
                        {
                            tcs.SetResult(default);
                        }
                    }
                    else
                    {
                        var error = root.TryGetProperty("error", out var errProp)
                            ? errProp.GetRawText()
                            : "Unknown error";
                        tcs.SetException(new Exception($"HA error: {error}"));
                    }
                }
            }
        }

        private void HandleEvent(JsonElement root)
        {
            try
            {
                if (!root.TryGetProperty("event", out var evt))
                {
                    return;
                }

                if (!evt.TryGetProperty("data", out var data))
                {
                    return;
                }

                if (!data.TryGetProperty("entity_id", out var entityIdProp))
                {
                    return;
                }

                var entityId = entityIdProp.GetString();

                HaEntity newState = null;
                HaEntity oldState = null;

                if (data.TryGetProperty("new_state", out var ns))
                {
                    newState = JsonSerializer.Deserialize<HaEntity>(ns.GetRawText());
                }

                if (data.TryGetProperty("old_state", out var os))
                {
                    oldState = JsonSerializer.Deserialize<HaEntity>(os.GetRawText());
                }

                if (newState != null)
                {
                    _states[entityId] = newState;
                }

                this.StateChanged?.Invoke(this, new HaStateChangedEventArgs
                {
                    EntityId = entityId,
                    NewState = newState,
                    OldState = oldState,
                });
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Error handling event");
            }
        }

        public async Task<JsonElement> SendCommandAsync(String type, Object additionalData = null)
        {
            var id = Interlocked.Increment(ref _messageId);
            var tcs = new TaskCompletionSource<JsonElement>();
            _pending[id] = tcs;

            var msg = new Dictionary<String, Object>
            {
                ["id"] = id,
                ["type"] = type,
            };

            if (additionalData != null)
            {
                var extra = JsonSerializer.Serialize(additionalData);
                var extraDict = JsonSerializer.Deserialize<Dictionary<String, JsonElement>>(extra);
                foreach (var kv in extraDict)
                {
                    msg[kv.Key] = kv.Value;
                }
            }

            var json = JsonSerializer.Serialize(msg);
            await this.SendRawAsync(json);

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            timeout.Token.Register(() => tcs.TrySetException(new TimeoutException("HA command timeout")));

            return await tcs.Task;
        }

        public async Task CallServiceAsync(String domain, String service, String entityId, Object serviceData = null)
        {
            var data = new Dictionary<String, Object>
            {
                ["type"] = "call_service",
                ["domain"] = domain,
                ["service"] = service,
                ["target"] = new { entity_id = entityId },
            };

            if (serviceData != null)
            {
                data["service_data"] = serviceData;
            }

            var id = Interlocked.Increment(ref _messageId);
            data["id"] = id;

            var tcs = new TaskCompletionSource<JsonElement>();
            _pending[id] = tcs;

            var json = JsonSerializer.Serialize(data);
            await this.SendRawAsync(json);

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            timeout.Token.Register(() => tcs.TrySetCanceled());

            try
            {
                await tcs.Task;
            }
            catch (TaskCanceledException)
            {
                PluginLog.Warning($"Service call timeout: {domain}.{service} for {entityId}");
            }
        }

        private async Task SendRawAsync(String message)
        {
            if (_ws?.State != WebSocketState.Open)
            {
                return;
            }

            await _sendLock.WaitAsync();
            try
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _ws.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    _cts?.Token ?? CancellationToken.None);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private void ScheduleReconnect()
        {
            if (_disposed)
            {
                return;
            }

            _reconnectTask = Task.Run(async () =>
            {
                while (!_disposed)
                {
                    await Task.Delay(ReconnectDelayMs);
                    try
                    {
                        PluginLog.Info("Attempting reconnection...");
                        await this.ConnectInternalAsync();
                        return;
                    }
                    catch (Exception ex)
                    {
                        PluginLog.Error(ex, "Reconnection failed, retrying...");
                    }
                }
            });
        }

        public void Dispose()
        {
            _disposed = true;
            _cts?.Cancel();
            try
            {
                if (_ws?.State == WebSocketState.Open)
                {
                    _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Plugin unloading", CancellationToken.None)
                        .GetAwaiter().GetResult();
                }
            }
            catch { }
            _ws?.Dispose();
            _cts?.Dispose();
            _sendLock?.Dispose();
        }
    }
}
