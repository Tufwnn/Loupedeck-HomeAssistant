namespace Loupedeck.HomeAssistantPlugin.Commands
{
    using System;

    public class MediaPlayerCommand : PluginDynamicCommand
    {
        private new HomeAssistantPlugin Plugin => (HomeAssistantPlugin)base.Plugin;

        public MediaPlayerCommand()
            : base()
        {
            this.IsWidget = true;
        }

        protected override Boolean OnLoad()
        {
            this.Plugin.HaStatesLoaded += this.OnStatesLoaded;
            this.Plugin.EntityStateChanged += this.OnEntityStateChanged;
            return true;
        }

        protected override Boolean OnUnload()
        {
            this.Plugin.HaStatesLoaded -= this.OnStatesLoaded;
            this.Plugin.EntityStateChanged -= this.OnEntityStateChanged;
            return true;
        }

        private void OnStatesLoaded(Object sender, EventArgs e) => this.RefreshParameters();

        private void RefreshParameters()
        {
            if (this.Plugin.HaClient == null)
            {
                return;
            }

            var entities = this.Plugin.HaClient.GetEntitiesByDomain("media_player");
            foreach (var entity in entities)
            {
                this.AddParameter(entity.EntityId, entity.FriendlyName, "Media###Play/Pause");
            }

            this.ParametersChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            if (String.IsNullOrEmpty(actionParameter) || this.Plugin.HaClient == null)
            {
                return;
            }

            var entity = this.Plugin.HaClient.GetEntity(actionParameter);
            if (entity == null)
            {
                return;
            }

            var service = entity.State == "playing" ? "media_pause" : "media_play";
            this.Plugin.HaClient.CallServiceAsync("media_player", service, actionParameter);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (String.IsNullOrEmpty(actionParameter))
            {
                return IconHelper.CreateOfflineImage(imageSize);
            }

            var entity = this.Plugin.HaClient?.GetEntity(actionParameter);
            if (entity == null)
            {
                return IconHelper.CreateOfflineImage(imageSize);
            }

            var isPlaying = entity.State == "playing";
            var icon = isPlaying ? "\u23F8" : "\u25B6";

            return IconHelper.CreateEntityImage(imageSize, entity.FriendlyName, entity.State, isPlaying, icon);
        }

        private void OnEntityStateChanged(Object sender, HaStateChangedEventArgs e)
        {
            if (e.NewState?.Domain == "media_player")
            {
                this.ActionImageChanged(e.EntityId);
            }
        }
    }
}
