namespace Loupedeck.HomeAssistantByBatuPlugin
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    internal class AdjustmentDebouncer<T> : IDisposable
    {
        private readonly ConcurrentDictionary<String, DebounceEntry> _entries = new();
        private readonly Action<String, T> _onFlush;
        private readonly Int32 _delayMs;

        public AdjustmentDebouncer(Action<String, T> onFlush, Int32 delayMs = 150)
        {
            _onFlush = onFlush;
            _delayMs = delayMs;
        }

        public T Accumulate(String key, T currentValue, Func<T, T> transform)
        {
            var entry = _entries.AddOrUpdate(
                key,
                _ =>
                {
                    var e = new DebounceEntry { Value = transform(currentValue) };
                    e.Timer = new Timer(this.FlushCallback, key, _delayMs, Timeout.Infinite);
                    return e;
                },
                (_, existing) =>
                {
                    existing.Value = transform(existing.Value);
                    existing.Timer?.Change(_delayMs, Timeout.Infinite);
                    return existing;
                });

            return entry.Value;
        }

        public Boolean TryGetPending(String key, out T value)
        {
            if (_entries.TryGetValue(key, out var entry))
            {
                value = entry.Value;
                return true;
            }

            value = default;
            return false;
        }

        private void FlushCallback(Object state)
        {
            var key = (String)state;
            if (_entries.TryRemove(key, out var entry))
            {
                entry.Timer?.Dispose();
                try
                {
                    _onFlush(key, entry.Value);
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, $"Debounce flush error for {key}");
                }
            }
        }

        public void Dispose()
        {
            foreach (var kv in _entries)
            {
                kv.Value.Timer?.Dispose();
            }

            _entries.Clear();
        }

        private class DebounceEntry
        {
            public T Value;
            public Timer Timer;
        }
    }
}
