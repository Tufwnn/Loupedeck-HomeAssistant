namespace Loupedeck.HomeAssistantByBatuPlugin.Adjustments
{
    using System;

    public class ClimateAdjustment : PluginDynamicAdjustment
    {
        private new HomeAssistantByBatuPlugin Plugin => (HomeAssistantByBatuPlugin)base.Plugin;

        private const Double TemperatureStep = 0.5;

        public ClimateAdjustment()
            : base(hasReset: false)
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

            var entities = this.Plugin.HaClient.GetEntitiesByDomain("climate");
            foreach (var entity in entities)
            {
                this.AddParameter(entity.EntityId, entity.FriendlyName, "Climate");
            }

            this.ParametersChanged();
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
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

            var currentTemp = entity.GetTemperature();
            var newTemp = currentTemp + (diff * TemperatureStep);

            this.Plugin.HaClient.CallServiceAsync("climate", "set_temperature", actionParameter,
                new { temperature = newTemp });

            this.AdjustmentValueChanged(actionParameter);
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

            var service = entity.State == "off" ? "turn_on" : "turn_off";
            this.Plugin.HaClient.CallServiceAsync("climate", service, actionParameter);
            this.ActionImageChanged(actionParameter);
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            if (String.IsNullOrEmpty(actionParameter))
            {
                return "";
            }

            var entity = this.Plugin.HaClient?.GetEntity(actionParameter);
            if (entity == null)
            {
                return "";
            }

            var temp = entity.GetTemperature();
            return temp > 0 ? $"{temp:F1}\u00B0" : entity.State;
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

            var isOn = entity.State != "off";
            var temp = entity.GetTemperature();
            var valueText = temp > 0 ? $"{temp:F1}\u00B0" : entity.State;

            return IconHelper.CreateAdjustmentImage(imageSize, entity.FriendlyName, valueText, isOn);
        }

        private void OnEntityStateChanged(Object sender, HaStateChangedEventArgs e)
        {
            if (e.NewState?.Domain == "climate")
            {
                this.AdjustmentValueChanged(e.EntityId);
                this.ActionImageChanged(e.EntityId);
            }
        }
    }
}
