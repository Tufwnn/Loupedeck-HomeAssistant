namespace Loupedeck.HomeAssistantPlugin
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal class PluginConfig
    {
        [JsonPropertyName("token")]
        public String Token { get; set; }

        [JsonPropertyName("url")]
        public String Url { get; set; }

        public static PluginConfig Load()
        {
            var configPath = GetConfigPath();
            if (!File.Exists(configPath))
            {
                PluginLog.Warning($"Config file not found at: {configPath}");
                return null;
            }

            try
            {
                var json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<PluginConfig>(json);

                if (String.IsNullOrWhiteSpace(config?.Token) || String.IsNullOrWhiteSpace(config?.Url))
                {
                    PluginLog.Error("Config file missing 'token' or 'url'");
                    return null;
                }

                config.Url = NormalizeUrl(config.Url);
                PluginLog.Info($"Config loaded. URL: {config.Url}");
                return config;
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed to load config");
                return null;
            }
        }

        private static String GetConfigPath()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, ".loupedeck", "homeassistant", "homeassistant.json");
        }

        private static String NormalizeUrl(String url)
        {
            url = url.TrimEnd('/');

            if (url.Contains("/api/websocket"))
            {
                return url;
            }

            var uri = new Uri(url);
            var scheme = uri.Scheme == "https" ? "wss" : "ws";
            var port = uri.IsDefaultPort ? "" : $":{uri.Port}";
            return $"{scheme}://{uri.Host}{port}/api/websocket";
        }
    }
}
