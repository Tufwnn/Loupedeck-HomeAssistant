namespace Loupedeck.HomeAssistantByBatuPlugin
{
    using System;
    using System.Drawing;

    internal static class IconHelper
    {
        public static BitmapImage CreateEntityImage(
            PluginImageSize imageSize,
            String displayText,
            String stateText,
            Boolean isOn,
            String iconChar = null)
        {
            using var builder = new BitmapBuilder(imageSize);

            var bgColor = isOn
                ? new BitmapColor(40, 120, 200)
                : new BitmapColor(60, 60, 60);

            builder.Clear(bgColor);

            var textColor = BitmapColor.White;

            if (!String.IsNullOrEmpty(iconChar))
            {
                builder.DrawText(iconChar, 0, 5, builder.Width, 30, textColor, 22);
            }

            var nameY = String.IsNullOrEmpty(iconChar) ? 10 : 32;
            var truncatedName = displayText?.Length > 14
                ? displayText.Substring(0, 12) + ".."
                : displayText;

            builder.DrawText(truncatedName ?? "", 2, nameY, builder.Width - 4, 24, textColor);

            if (!String.IsNullOrEmpty(stateText))
            {
                var stateColor = isOn
                    ? new BitmapColor(200, 255, 200)
                    : new BitmapColor(180, 180, 180);
                builder.DrawText(stateText, 2, nameY + 22, builder.Width - 4, 20, stateColor, 11);
            }

            return builder.ToImage();
        }

        public static BitmapImage CreateAdjustmentImage(
            PluginImageSize imageSize,
            String displayText,
            String valueText,
            Boolean isOn)
        {
            using var builder = new BitmapBuilder(imageSize);

            var bgColor = isOn
                ? new BitmapColor(40, 120, 200)
                : new BitmapColor(60, 60, 60);

            builder.Clear(bgColor);

            var textColor = BitmapColor.White;

            var truncatedName = displayText?.Length > 14
                ? displayText.Substring(0, 12) + ".."
                : displayText;

            builder.DrawText(truncatedName ?? "", 2, 8, builder.Width - 4, 24, textColor);

            if (!String.IsNullOrEmpty(valueText))
            {
                builder.DrawText(valueText, 2, 34, builder.Width - 4, 28, new BitmapColor(255, 255, 100), 16);
            }

            return builder.ToImage();
        }

        public static BitmapImage CreateColorTempImage(
            PluginImageSize imageSize,
            String displayText,
            String valueText,
            Boolean isOn,
            Double warmthFactor)
        {
            using var builder = new BitmapBuilder(imageSize);

            BitmapColor bgColor;
            if (!isOn)
            {
                bgColor = new BitmapColor(60, 60, 60);
            }
            else
            {
                var r = (Int32)(180 + 75 * warmthFactor);
                var g = (Int32)(160 + 50 * (1 - warmthFactor));
                var b = (Int32)(80 + 175 * (1 - warmthFactor));
                bgColor = new BitmapColor(
                    Math.Min(r, 255),
                    Math.Min(g, 255),
                    Math.Min(b, 255));
            }

            builder.Clear(bgColor);

            var textColor = isOn ? BitmapColor.Black : BitmapColor.White;

            var truncatedName = displayText?.Length > 14
                ? displayText.Substring(0, 12) + ".."
                : displayText;

            builder.DrawText(truncatedName ?? "", 2, 8, builder.Width - 4, 24, textColor);

            if (!String.IsNullOrEmpty(valueText))
            {
                var valueColor = isOn
                    ? new BitmapColor(40, 40, 40)
                    : new BitmapColor(255, 255, 100);
                builder.DrawText(valueText, 2, 34, builder.Width - 4, 28, valueColor, 16);
            }

            return builder.ToImage();
        }

        public static BitmapImage CreateOfflineImage(PluginImageSize imageSize)
        {
            using var builder = new BitmapBuilder(imageSize);
            builder.Clear(new BitmapColor(80, 20, 20));
            builder.DrawText("Home", 0, 14, builder.Width, 20, BitmapColor.White);
            builder.DrawText("Assistant", 0, 32, builder.Width, 20, BitmapColor.White);
            builder.DrawText("Offline", 0, 50, builder.Width, 16, new BitmapColor(255, 100, 100), 10);
            return builder.ToImage();
        }

        public static String GetDomainIcon(String domain)
        {
            return domain switch
            {
                "light" => "\u2600",        // ☀
                "switch" => "\u26A1",       // ⚡
                "automation" => "\u2699",   // ⚙
                "scene" => "\u2B50",        // ⭐
                "script" => "\u25B6",       // ▶
                "button" => "\u25CF",       // ●
                "lock" => "\uD83D\uDD12",  // 🔒
                "cover" => "\u2195",        // ↕
                "climate" => "\uD83C\uDF21", // 🌡
                "sensor" => "\uD83D\uDCCA", // 📊
                "binary_sensor" => "\u26AB", // ⚫
                "fan" => "\uD83C\uDF00",    // 🌀
                "media_player" => "\u266B",  // ♫
                "camera" => "\uD83D\uDCF7",  // 📷
                "water_heater" => "\uD83D\uDCA7", // 💧
                _ => "\u2B24",               // ⬤
            };
        }
    }
}
