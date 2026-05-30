using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.EnhancedAuth.Configuration;

public class EnhancedRulesetConfigManager(SettingsStore store, RulesetInfo ruleset, int? variant = null)
    : RulesetConfigManager<EnhancedRulesetSettings>(store,
        ruleset, variant)
{
    protected override void InitialiseDefaults()
    {
        base.InitialiseDefaults();

        SetDefault(EnhancedRulesetSettings.ApiUrl, string.Empty);
        SetDefault(EnhancedRulesetSettings.WebsiteUrl, string.Empty);
        SetDefault(EnhancedRulesetSettings.ClientId, string.Empty);
        SetDefault(EnhancedRulesetSettings.ClientSecret, string.Empty);
        SetDefault(EnhancedRulesetSettings.SpectatorUrl, string.Empty);
        SetDefault(EnhancedRulesetSettings.MultiplayerUrl, string.Empty);
        SetDefault(EnhancedRulesetSettings.MetadataUrl, string.Empty);
        SetDefault(EnhancedRulesetSettings.BeatmapSubmissionServiceUrl, string.Empty);
        SetDefault(EnhancedRulesetSettings.DisableSentryLogger, true);
        SetDefault(EnhancedRulesetSettings.NonG0V0Server, false);
    }
}

public enum EnhancedRulesetSettings
{
    ApiUrl,
    WebsiteUrl,
    ClientId,
    ClientSecret,
    SpectatorUrl,
    MultiplayerUrl,
    MetadataUrl,
    BeatmapSubmissionServiceUrl,
    DisableSentryLogger,
    NonG0V0Server,
}
