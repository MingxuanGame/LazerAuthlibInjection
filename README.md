# AuthlibInjection

A ruleset to add custom server support for osu!lazer.

## Availability

Linux (amd64, arm64), Windows (amd64, arm64), macOS (Intel, Apple Silicon)

## Quick Start

1. Navigate to `osu` data directory

In the setting panel, click `Open osu! folder` to open the osu! data directory.

Default locations:

- Windows: `%AppData%\osu!`
- Linux: `~/.local/share/osu`
- macOS: `~/Library/Application Support/osu`

2. Install AuthlibInjection

Download the latest release from the [Releases](https://github.com/MingxuanGame/LazerAuthlibInjection/releases) and copy
`osu.Game.Rulesets.AuthlibInjection.dll` into the `rulesets` directory.

3. Configure AuthlibInjection

Click `Rulesets` in the setting panel and configure `API Url` and `Website Url` to your custom server's API and web
URLs.

## Pass through settings by command line

You can also pass through the settings by command line arguments:

- `--api-url` / `-devserver`: API Url
- `--website-url`: Website Url
- `--client-id`: Client ID
- `--client-secret`: Client Secret
- `--spectator-url`: Spectator Url
- `--multiplayer-url`: Multiplayer Url
- `--metadata-url`: Metadata Url
- `--bss-url`: Beatmap Submission Service Url
- `--disable-sentry-logger`: Disable Sentry to osu! team
- `--non-g0v0-server`: Disable specific features for [g0v0-server](https://github.com/GooGuTeam/g0v0-server) like Relax
  functions & submitting custom ruleset scores.

Example:

```bash
osu!.exe --api-url=lazer-api.g0v0.top --website-url=lazer.g0v0.top --disable-sentry-logger
```

## Disclaimer

This project is not affiliated with or endorsed by the official osu! team. Use it at your own risk.

AuthlibInjection is provided "as is" without warranty of any kind. The author is not responsible for any damage or
account banning that may occur from using this ruleset.

**DO NOT report any issue about this ruleset to osu! team**

## License

AGPL-3.0 License. See [LICENSE](LICENSE) for details.
