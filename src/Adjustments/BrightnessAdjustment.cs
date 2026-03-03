namespace Loupedeck.HomeAssistantByBatuPlugin.Adjustments
{
    using System;

    public class BrightnessAdjustment : PluginDynamicAdjustment
    {
        private new HomeAssistantByBatuPlugin Plugin => (HomeAssistantByBatuPlugin)base.Plugin;

        private const Int32 BrightnessStep = 10;

        public BrightnessAdjustment()
            : base(hasReset: true)
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

            var entities = this.Plugin.HaClient.GetEntitiesByDomain("light");
            foreach (var entity in entities)
            {
                this.AddParameter(entity.EntityId, entity.FriendlyName, "Dimmers");
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

            var currentPct = entity.GetBrightnessPercent();
            var newPct = Math.Clamp(currentPct + (diff * BrightnessStep), 0, 100);

            if (newPct == 0)
            {
                this.Plugin.HaClient.CallServiceAsync("light", "turn_off", actionParameter);
            }
            else
            {
                var brightness = (Int32)Math.Round(newPct / 100.0 * 255);
                this.Plugin.HaClient.CallServiceAsync("light", "turn_on", actionParameter,
                    new { brightness });
            }

            this.AdjustmentValueChanged(actionParameter);
        }

        protected override void RunCommand(String actionParameter)
        {
            if (String.IsNullOrEmpty(actionParameter) || this.Plugin.HaClient == null)
            {
                return;
            }

            this.Plugin.HaClient.CallServiceAsync("light", "toggle", actionParameter);
            this.ActionImageChanged(actionParameter);
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            if (String.IsNullOrEmpty(actionParameter))
            {
                return "";
            }

            var entity = this.Plugin.HaClient?.GetEntity(actionParameter);
            if (entity == null || !entity.IsOn)
            {
                return "OFF";
            }

            return $"{entity.GetBrightnessPercent()}%";
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

            var valueText = entity.IsOn ? $"{entity.GetBrightnessPercent()}%" : "OFF";
            return IconHelper.CreateAdjustmentImage(imageSize, entity.FriendlyName, valueText, entity.IsOn);
        }

        private void OnEntityStateChanged(Object sender, HaStateChangedEventArgs e)
        {
            if (e.NewState?.Domain == "light")
            {
                this.AdjustmentValueChanged(e.EntityId);
                this.ActionImageChanged(e.EntityId);
            }
        }
    }
}
