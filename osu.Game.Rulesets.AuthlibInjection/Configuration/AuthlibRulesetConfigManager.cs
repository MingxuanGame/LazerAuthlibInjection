using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.AuthlibInjection.Configuration;

public class AuthlibRulesetConfigManager(SettingsStore store, RulesetInfo ruleset, int? variant = null)
    : RulesetConfigManager<AuthlibRulesetSettings>(store,
        ruleset, variant)
{
    protected override void InitialiseDefaults()
    {
        base.InitialiseDefaults();

        SetDefault(AuthlibRulesetSettings.ApiUrl, string.Empty);
        SetDefault(AuthlibRulesetSettings.WebsiteUrl, string.Empty);
        SetDefault(AuthlibRulesetSettings.ClientId, string.Empty);
        SetDefault(AuthlibRulesetSettings.ClientSecret, string.Empty);
        SetDefault(AuthlibRulesetSettings.SpectatorUrl, string.Empty);
        SetDefault(AuthlibRulesetSettings.MultiplayerUrl, string.Empty);
        SetDefault(AuthlibRulesetSettings.MetadataUrl, string.Empty);
        SetDefault(AuthlibRulesetSettings.BeatmapSubmissionServiceUrl, string.Empty);
    }
}

public enum AuthlibRulesetSettings
{
    ApiUrl,
    WebsiteUrl,
    ClientId,
    ClientSecret,
    SpectatorUrl,
    MultiplayerUrl,
    MetadataUrl,
    BeatmapSubmissionServiceUrl
}
