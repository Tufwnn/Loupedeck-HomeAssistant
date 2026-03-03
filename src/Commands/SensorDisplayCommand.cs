namespace Loupedeck.HomeAssistantByBatuPlugin.Commands
{
    using System;
    using System.Linq;

    public class SensorDisplayCommand : PluginDynamicCommand
    {
        private new HomeAssistantByBatuPlugin Plugin => (HomeAssistantByBatuPlugin)base.Plugin;

        private static readonly String[] SensorDomains = new[] { "sensor", "binary_sensor", "water_heater" };

        public SensorDisplayCommand()
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

            foreach (var domain in SensorDomains)
            {
                var entities = this.Plugin.HaClient.GetEntitiesByDomain(domain);
                var groupName = domain switch
                {
                    "sensor" => "Sensors###Sensors",
                    "binary_sensor" => "Sensors###Binary Sensors",
                    "water_heater" => "Sensors###Water Heater",
                    _ => "Sensors",
                };

                foreach (var entity in entities)
                {
                    this.AddParameter(entity.EntityId, entity.FriendlyName, groupName);
                }
            }

            this.ParametersChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            if (!String.IsNullOrEmpty(actionParameter))
            {
                this.ActionImageChanged(actionParameter);
            }
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
            var unit = entity.GetUnitOfMeasurement();
            var stateText = String.IsNullOrEmpty(unit) ? entity.State : $"{entity.State} {unit}";

            var isActive = entity.Domain == "binary_sensor" ? entity.IsOn : !String.IsNullOrEmpty(entity.State);

            return IconHelper.CreateEntityImage(imageSize, entity.FriendlyName, stateText, isActive, icon);
        }

        private void OnEntityStateChanged(Object sender, HaStateChangedEventArgs e)
        {
            if (e.NewState != null && SensorDomains.Contains(e.NewState.Domain))
            {
                this.ActionImageChanged(e.EntityId);
            }
        }
    }
}
