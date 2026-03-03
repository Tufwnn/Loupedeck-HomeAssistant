namespace Loupedeck.HomeAssistantPlugin
{
    using System;

    public class HomeAssistantPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;
        public override Boolean HasNoApplication => true;

        internal HomeAssistantClient HaClient { get; private set; }
        internal PluginConfig Config { get; private set; }
        internal Boolean IsConfigured => this.Config != null;
        internal Boolean IsReady => this.HaClient?.IsConnected == true && this.HaClient?.States.Count > 0;

        public HomeAssistantPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
            try
            {
                PluginLog.Info("Loading Home Assistant by Batu plugin...");

                this.Config = PluginConfig.Load();
                if (this.Config == null)
                {
                    PluginLog.Warning("No configuration found. Create homeassistant.json in ~/.loupedeck/homeassistant/");
                    return;
                }

                PluginLog.Info($"Connecting to: {this.Config.Url}");
                this.HaClient = new HomeAssistantClient(this.Config.Url, this.Config.Token);
                this.HaClient.StateChanged += this.OnStateChanged;
                this.HaClient.Connected += this.OnConnected;
                this.HaClient.Disconnected += this.OnDisconnected;
                this.HaClient.StatesLoaded += this.OnStatesLoaded;
                this.HaClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Fatal error in Load()");
            }
        }

        public override void Unload()
        {
            if (this.HaClient != null)
            {
                this.HaClient.StateChanged -= this.OnStateChanged;
                this.HaClient.Connected -= this.OnConnected;
                this.HaClient.Disconnected -= this.OnDisconnected;
                this.HaClient.StatesLoaded -= this.OnStatesLoaded;
                this.HaClient.Dispose();
                this.HaClient = null;
            }
        }

        public event EventHandler<HaStateChangedEventArgs> EntityStateChanged;
        public event EventHandler HaConnected;
        public event EventHandler HaDisconnected;
        public event EventHandler HaStatesLoaded;

        private void OnStateChanged(Object sender, HaStateChangedEventArgs e)
        {
            this.EntityStateChanged?.Invoke(this, e);
        }

        private void OnConnected(Object sender, EventArgs e)
        {
            PluginLog.Info("Connected to Home Assistant");
            this.HaConnected?.Invoke(this, e);
        }

        private void OnDisconnected(Object sender, EventArgs e)
        {
            PluginLog.Warning("Disconnected from Home Assistant");
            this.HaDisconnected?.Invoke(this, e);
        }

        private void OnStatesLoaded(Object sender, EventArgs e)
        {
            PluginLog.Info($"States loaded - {this.HaClient.States.Count} entities available");
            this.HaStatesLoaded?.Invoke(this, e);
        }
    }
}
