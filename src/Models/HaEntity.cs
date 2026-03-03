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

        public Int32 GetColorTemp()
        {
            try
            {
                if (this.Attributes.ValueKind == JsonValueKind.Object &&
                    this.Attributes.TryGetProperty("color_temp", out var ct))
                {
                    return ct.GetInt32();
                }

                if (this.Attributes.ValueKind == JsonValueKind.Object &&
                    this.Attributes.TryGetProperty("color_temp_kelvin", out var ctk))
                {
                    return ctk.GetInt32();
                }
            }
            catch { }
            return 0;
        }

        public Boolean SupportsColorTemp()
        {
            try
            {
                if (this.Attributes.ValueKind != JsonValueKind.Object)
                {
                    return false;
                }

                if (this.Attributes.TryGetProperty("supported_color_modes", out var modes) &&
                    modes.ValueKind == JsonValueKind.Array)
                {
                    foreach (var mode in modes.EnumerateArray())
                    {
                        var m = mode.GetString();
                        if (m == "color_temp")
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public (Int32 min, Int32 max) GetColorTempRange()
        {
            var min = 153;
            var max = 500;

            try
            {
                if (this.Attributes.ValueKind == JsonValueKind.Object)
                {
                    if (this.Attributes.TryGetProperty("min_mireds", out var minProp))
                    {
                        min = minProp.GetInt32();
                    }

                    if (this.Attributes.TryGetProperty("max_mireds", out var maxProp))
                    {
                        max = maxProp.GetInt32();
                    }

                    if (this.Attributes.TryGetProperty("min_color_temp_kelvin", out var minK) &&
                        this.Attributes.TryGetProperty("max_color_temp_kelvin", out var maxK))
                    {
                        min = (Int32)Math.Round(1000000.0 / maxK.GetInt32());
                        max = (Int32)Math.Round(1000000.0 / minK.GetInt32());
                    }
                }
            }
            catch { }

            return (min, max);
        }

        public Int32 GetColorTempPercent()
        {
            var ct = this.GetColorTemp();
            if (ct <= 0)
            {
                return 50;
            }

            var (min, max) = this.GetColorTempRange();
            if (max <= min)
            {
                return 50;
            }

            return (Int32)Math.Round((ct - min) * 100.0 / (max - min));
        }
    }

    public class HaStateChangedEventArgs : EventArgs
    {
        public String EntityId { get; set; }
        public HaEntity NewState { get; set; }
        public HaEntity OldState { get; set; }
    }
}
