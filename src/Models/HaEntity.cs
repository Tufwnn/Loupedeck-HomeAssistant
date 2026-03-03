namespace Loupedeck.HomeAssistantByBatuPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class HaEntity
    {
        [JsonPropertyName("entity_id")]
        public String EntityId { get; set; }

        [JsonPropertyName("state")]
        public String State { get; set; }

        [JsonPropertyName("attributes")]
        public JsonElement Attributes { get; set; }

        public String FriendlyName
        {
            get
            {
                try
                {
                    if (this.Attributes.ValueKind == JsonValueKind.Object &&
                        this.Attributes.TryGetProperty("friendly_name", out var name))
                    {
                        return name.GetString();
                    }
                }
                catch { }
                return this.EntityId;
            }
        }

        public String Domain => this.EntityId?.Split('.')[0] ?? "";

        public Boolean IsOn => String.Equals(this.State, "on", StringComparison.OrdinalIgnoreCase);

        public Int32 GetBrightness()
        {
            try
            {
                if (this.Attributes.ValueKind == JsonValueKind.Object &&
                    this.Attributes.TryGetProperty("brightness", out var b))
                {
                    return b.GetInt32();
                }
            }
            catch { }
            return 0;
        }

        public Int32 GetBrightnessPercent()
        {
            var raw = this.GetBrightness();
            return (Int32)Math.Round(raw / 255.0 * 100);
        }

        public Double GetTemperature()
        {
            try
            {
                if (this.Attributes.ValueKind == JsonValueKind.Object &&
                    this.Attributes.TryGetProperty("temperature", out var t))
                {
                    return t.GetDouble();
                }
            }
            catch { }
            return 0;
        }

        public Int32 GetPosition()
        {
            try
            {
                if (this.Attributes.ValueKind == JsonValueKind.Object &&
                    this.Attributes.TryGetProperty("current_position", out var p))
                {
                    return p.GetInt32();
                }
            }
            catch { }
            return 0;
        }

        public String GetUnitOfMeasurement()
        {
            try
            {
                if (this.Attributes.ValueKind == JsonValueKind.Object &&
                    this.Attributes.TryGetProperty("unit_of_measurement", out var u))
                {
                    return u.GetString();
                }
            }
            catch { }
            return "";
        }
    }

    public class HaStateChangedEventArgs : EventArgs
    {
        public String EntityId { get; set; }
        public HaEntity NewState { get; set; }
        public HaEntity OldState { get; set; }
    }
}
