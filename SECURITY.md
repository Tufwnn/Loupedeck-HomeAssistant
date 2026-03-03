# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability in this plugin, please **do not** open a public issue.

Instead, please email **batukeanos@outlook.com** with:

- A description of the vulnerability
- Steps to reproduce
- Any potential impact

I will respond as soon as possible and work on a fix.

## Security Notes

- **Never commit** your Home Assistant access token or any credentials to this repository.
- The `homeassistant.json` config file is in `.gitignore` and should stay local at `~/.loupedeck/homeassistant/homeassistant.json`.
- The plugin communicates with Home Assistant only over the local network via WebSocket.
- No data is sent to any external/third-party server.
