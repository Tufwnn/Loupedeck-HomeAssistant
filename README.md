# Home Assistant by Batu — Loupedeck Plugin

[![Release](https://img.shields.io/github/v/release/Batushn/Loupedeck-HomeAssistant?style=flat-square)](https://github.com/Batushn/Loupedeck-HomeAssistant/releases/latest)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)

Control your **Home Assistant** devices directly from your Loupedeck or Razer Stream Controller.
Real-time WebSocket connection — entity states update on your device instantly.

> **Heads up:** This plugin was vibecoded (built with heavy AI assistance). It works, but there may be rough edges, undiscovered bugs, or missing features. PRs and issues are very welcome!

---

## Installation

### Step 1 — Download

Go to [**Releases**](https://github.com/Batushn/Loupedeck-HomeAssistant/releases/latest) and download `HomeAssistantByBatuPlugin.lplug4`.

### Step 2 — Install the plugin

**Option A (recommended):** Double-click the `.lplug4` file. Loupedeck software will install it automatically.

**Option B (manual):** Run the included `install.ps1` script:

```powershell
powershell -ExecutionPolicy Bypass -File install.ps1
```

### Step 3 — Create your config file

Create this file on your computer:

**Windows:**
```
C:\Users\<YOUR_USERNAME>\.loupedeck\homeassistant\homeassistant.json
```

**macOS:**
```
~/.loupedeck/homeassistant/homeassistant.json
```

With this content:

```json
{
  "token": "YOUR_LONG_LIVED_ACCESS_TOKEN",
  "url": "http://YOUR_HA_IP:8123"
}
```

#### How to get the token

1. Open Home Assistant in your browser
2. Click your profile (bottom-left corner)
3. Scroll down to **Long-Lived Access Tokens**
4. Click **Create Token**, give it a name, copy the token

#### URL tips

- Use your Home Assistant's **IP address** (e.g., `http://192.168.1.100:8123`)
- `homeassistant.local` may not resolve on all systems — IP is more reliable
- The plugin auto-converts HTTP to WebSocket, so just use your normal HA URL

### Step 4 — Use it

1. Open Loupedeck / Logi Options+ software
2. Find **Home Assistant** in the plugin list
3. Drag & drop entities onto your buttons and dials

---

## Supported Devices

- Loupedeck CT
- Loupedeck Live / Live S
- Razer Stream Controller / Stream Controller X

## What You Can Control

### Buttons

| Entity Type | Action |
|---|---|
| **Lights** | Toggle on/off |
| **Switches** | Toggle on/off |
| **Automations** | Toggle on/off |
| **Fans** | Toggle on/off |
| **Input Booleans** | Toggle on/off |
| **Locks** | Lock / Unlock |
| **Scenes** | Activate |
| **Scripts** | Run |
| **Buttons** | Press |
| **Media Players** | Play / Pause |
| **Sensors** | Display state (read-only) |
| **Binary Sensors** | Display state |

### Dials / Encoders

| Entity Type | Rotate | Press |
|---|---|---|
| **Lights (dimmable)** | Brightness ±10% | Toggle on/off |
| **Covers** | Position ±10% | Open / Close |
| **Climate** | Temperature ±0.5° | Toggle on/off |
| **Media Players** | Volume ±5% | Mute |

---

## Building from Source

### Requirements

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Loupedeck software or Logi Options+ installed (provides PluginApi.dll)

### Build

```bash
git clone https://github.com/Batushn/Loupedeck-HomeAssistant.git
cd Loupedeck-HomeAssistant

# Debug build — auto-creates .link file for hot reload
dotnet build src/HomeAssistantByBatuPlugin.csproj

# Release build — produces .lplug4 in output/
dotnet build src/HomeAssistantByBatuPlugin.csproj -c Release
```

Or just double-click `build.bat` on Windows.

### Project Structure

```
src/
├── HomeAssistantByBatuPlugin.cs   # Plugin entry point & lifecycle
├── HomeAssistantApplication.cs   # Application class (required by SDK)
├── Models/
│   ├── PluginConfig.cs           # Config loading (homeassistant.json)
│   └── HaEntity.cs               # Entity data model
├── Services/
│   └── HomeAssistantClient.cs    # WebSocket client + auto-reconnect
├── Commands/                     # Button actions
│   ├── ToggleEntityCommand.cs
│   ├── LockCommand.cs
│   ├── SceneCommand.cs
│   ├── ScriptCommand.cs
│   ├── ButtonPressCommand.cs
│   ├── MediaPlayerCommand.cs
│   ├── SensorDisplayCommand.cs
│   └── ConnectionStatusCommand.cs
├── Adjustments/                  # Dial/encoder actions
│   ├── BrightnessAdjustment.cs
│   ├── CoverAdjustment.cs
│   ├── ClimateAdjustment.cs
│   └── MediaVolumeAdjustment.cs
└── Helpers/
    ├── PluginLog.cs
    ├── PluginResources.cs
    └── IconHelper.cs
```

### Creating a Release

Push a version tag to trigger the GitHub Actions workflow:

```bash
git tag v1.1.0
git push origin v1.1.0
```

This builds the plugin and uploads the `.lplug4` to GitHub Releases automatically.

---

## Troubleshooting

| Problem | Solution |
|---|---|
| Plugin doesn't appear | Restart Logi Plugin Service |
| Can't connect | Check `homeassistant.json` — verify token and URL |
| No entities listed | Make sure your HA URL is reachable from this computer |
| Connection status stuck on "Connecting" | Use IP address instead of `homeassistant.local` |
| Where are the logs? | `%LOCALAPPDATA%\Logi\LogiPluginService\Logs\plugin_logs\` |

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). All PRs require maintainer review before merge.

## Security

See [SECURITY.md](SECURITY.md). Never commit your Home Assistant tokens.

## License

MIT — see [LICENSE](LICENSE)
