# Contributing

Thanks for your interest in contributing! Here's how you can help.

## How to Contribute

1. **Fork** this repository
2. Create a **feature branch** (`git checkout -b feature/my-feature`)
3. Make your changes
4. **Test** the plugin locally (build, install, verify it works)
5. **Commit** with clear messages
6. **Push** to your fork and open a **Pull Request**

## Pull Request Rules

- All PRs must target the `main` branch
- All PRs require review from a maintainer before merge
- Keep PRs focused — one feature or fix per PR
- Test your changes on an actual Loupedeck/Razer device if possible

## Development Setup

See the [README](README.md) for build instructions.

### Quick Start

```bash
git clone https://github.com/YOUR_USERNAME/Loupedeck-HomeAssistant.git
cd Loupedeck-HomeAssistant
dotnet build src/HomeAssistantByBatuPlugin.csproj
```

Debug builds auto-create a `.link` file so the Logi Plugin Service loads directly from your build output.

## Adding New Entity Types

1. Create a new command in `src/Commands/` or adjustment in `src/Adjustments/`
2. Follow the pattern of existing commands (e.g., `ToggleEntityCommand.cs`)
3. Filter entities by domain in `GetParameters()`
4. Handle the action in `RunCommand()` or the adjustment methods

## Code Style

- Use the existing code style (C# conventions, `System.` prefix for types)
- No unnecessary comments — code should be self-explanatory
- Keep classes focused and small

## Reporting Issues

- Use [GitHub Issues](https://github.com/Batushn/Loupedeck-HomeAssistant/issues)
- Include your Loupedeck software version, device model, and relevant logs
- Logs are at `%LOCALAPPDATA%\Logi\LogiPluginService\Logs\plugin_logs\`
