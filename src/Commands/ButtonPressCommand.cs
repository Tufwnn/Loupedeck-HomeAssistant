namespace Loupedeck.HomeAssistantByBatuPlugin.Commands
{
    using System;

    public class ButtonPressCommand : PluginDynamicCommand
    {
        private new HomeAssistantByBatuPlugin Plugin => (HomeAssistantByBatuPlugin)base.Plugin;

        public ButtonPressCommand()
            : base()
        {
            this.IsWidget = true;
        }

        protected override Boolean OnLoad()
        {
            this.Plugin.HaStatesLoaded += this.OnStatesLoaded;
            return true;
        }

        protected override Boolean OnUnload()
        {
            this.Plugin.HaStatesLoaded -= this.OnStatesLoaded;
            return true;
        }

        private void OnStatesLoaded(Object sender, EventArgs e) => this.RefreshParameters();

        private void RefreshParameters()
        {
            if (this.Plugin.HaClient == null)
            {
                return;
            }

            var entities = this.Plugin.HaClient.GetEntitiesByDomain("button");
            foreach (var entity in entities)
            {
                this.AddParameter(entity.EntityId, entity.FriendlyName, "Buttons");
            }

            var inputButtons = this.Plugin.HaClient.GetEntitiesByDomain("input_button");
            foreach (var entity in inputButtons)
            {
                this.AddParameter(entity.EntityId, entity.FriendlyName, "Buttons###Input Buttons");
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

            this.Plugin.HaClient.CallServiceAsync(entity.Domain, "press", actionParameter);
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

            return IconHelper.CreateEntityImage(imageSize, entity.FriendlyName, "Press", false, "\u25CF");
        }
    }
}
