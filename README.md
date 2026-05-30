# AuthlibInjection

A ruleset to add custom server support for osu!lazer.

## Disclaimer

> [!CAUTION]
> **This will result in your osu! account being banned!**
>
> The official osu! team has explicitly stated that using AuthlibInjection and similar injection rulesets to connect to official servers will result in account bans ([see this issue](https://github.com/Cai1Hsu/osu-plugins/issues/93)).
>
> Only use this tool on **private development servers**, and ensure you have logged out of or disconnected from the official servers before using it.

This project is not affiliated with or endorsed by the official osu! team. Use it at your own risk.

AuthlibInjection is provided "as is" without warranty of any kind. The author is not responsible for any damage or
account banning that may occur from using this ruleset.

**DO NOT report any issue about this ruleset to osu! team**

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
`osu.Game.Rulesets.EnhancedAuth.dll` into the `rulesets` directory.

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

## License

AGPL-3.0 License. See [LICENSE](LICENSE) for details.
