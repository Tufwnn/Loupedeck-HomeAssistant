namespace Loupedeck.HomeAssistantPlugin.Commands
{
    using System;
    using System.Linq;

    public class ToggleEntityCommand : PluginDynamicCommand
    {
        private new HomeAssistantPlugin Plugin => (HomeAssistantPlugin)base.Plugin;

        private static readonly String[] SupportedDomains = new[]
        {
            "light", "switch", "automation", "fan", "input_boolean"
        };

        public ToggleEntityCommand()
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

        private void OnStatesLoaded(Object sender, EventArgs e)
        {
            this.RefreshParameters();
        }

        private void RefreshParameters()
        {
            if (this.Plugin.HaClient == null)
            {
                return;
            }

            foreach (var domain in SupportedDomains)
            {
                var entities = this.Plugin.HaClient.GetEntitiesByDomain(domain);
                var groupName = GetGroupName(domain);

                foreach (var entity in entities)
                {
                    this.AddParameter(entity.EntityId, entity.FriendlyName, groupName);
                }
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

            this.Plugin.HaClient.CallServiceAsync(entity.Domain, "toggle", actionParameter);
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

            var icon = IconHelper.GetDomainIcon(entity.Domain);
            var stateText = entity.IsOn ? "ON" : "OFF";

            return IconHelper.CreateEntityImage(imageSize, entity.FriendlyName, stateText, entity.IsOn, icon);
        }

        private void OnEntityStateChanged(Object sender, HaStateChangedEventArgs e)
        {
            if (e.NewState != null && SupportedDomains.Contains(e.NewState.Domain))
            {
                this.ActionImageChanged(e.EntityId);
            }
        }

        private static String GetGroupName(String domain)
        {
            return domain switch
            {
                "light" => "Toggle###Lights",
                "switch" => "Toggle###Switches",
                "automation" => "Toggle###Automations",
                "fan" => "Toggle###Fans",
                "input_boolean" => "Toggle###Input Booleans",
                _ => "Toggle",
            };
        }
    }
}
