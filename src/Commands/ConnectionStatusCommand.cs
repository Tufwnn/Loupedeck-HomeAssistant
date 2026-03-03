namespace Loupedeck.HomeAssistantByBatuPlugin.Commands
{
    using System;

    public class ConnectionStatusCommand : PluginDynamicCommand
    {
        private new HomeAssistantByBatuPlugin Plugin => (HomeAssistantByBatuPlugin)base.Plugin;

        public ConnectionStatusCommand()
            : base("Connection Status", "Shows Home Assistant connection status", "Status")
        {
            this.IsWidget = true;
        }

        protected override Boolean OnLoad()
        {
            this.Plugin.HaConnected += (s, e) => this.ActionImageChanged();
            this.Plugin.HaDisconnected += (s, e) => this.ActionImageChanged();
            this.Plugin.HaStatesLoaded += (s, e) => this.ActionImageChanged();
            return true;
        }

        protected override void RunCommand(String actionParameter)
        {
            if (this.Plugin.HaClient != null && !this.Plugin.HaClient.IsConnected)
            {
                this.Plugin.HaClient.ConnectAsync();
            }
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            using var builder = new BitmapBuilder(imageSize);

            if (!this.Plugin.IsConfigured)
            {
                builder.Clear(new BitmapColor(120, 30, 30));
                builder.DrawText("Home", 0, 8, builder.Width, 18, BitmapColor.White, 12);
                builder.DrawText("Assistant", 0, 24, builder.Width, 18, BitmapColor.White, 12);
                builder.DrawText("---", 0, 42, builder.Width, 14, new BitmapColor(255, 200, 100), 10);
                builder.DrawText("No Config", 0, 56, builder.Width, 14, new BitmapColor(255, 100, 100), 9);
                return builder.ToImage();
            }

            if (this.Plugin.IsReady)
            {
                var count = this.Plugin.HaClient.States.Count;
                builder.Clear(new BitmapColor(20, 100, 50));
                builder.DrawText("Home", 0, 8, builder.Width, 18, BitmapColor.White, 12);
                builder.DrawText("Assistant", 0, 24, builder.Width, 18, BitmapColor.White, 12);
                builder.DrawText("\u2713 Online", 0, 42, builder.Width, 14, new BitmapColor(100, 255, 100), 10);
                builder.DrawText($"{count} entities", 0, 56, builder.Width, 14, new BitmapColor(200, 200, 200), 9);
                return builder.ToImage();
            }

            builder.Clear(new BitmapColor(100, 80, 20));
            builder.DrawText("Home", 0, 8, builder.Width, 18, BitmapColor.White, 12);
            builder.DrawText("Assistant", 0, 24, builder.Width, 18, BitmapColor.White, 12);
            builder.DrawText("Connecting...", 0, 44, builder.Width, 14, new BitmapColor(255, 255, 100), 10);
            return builder.ToImage();
        }
    }
}
