using osu.Game.Rulesets.EnhancedAuth.Extensions;

namespace osu.Game.Rulesets.EnhancedAuth.Configuration;

public class EnhancedRulesetConfig
{
    public const string CONFIG_FILE_NAME = "authlib_local_config.json";
    public EnhancedRulesetConfig() { }

    public EnhancedRulesetConfig(string apiUrl,
                                string websiteUrl,
                                string clientId,
                                string clientSecret,
                                string spectatorUrl,
                                string multiplayerUrl,
                                string metadataUrl,
                                string beatmapSubmissionServiceUrl,
                                bool disableSentryLogger, bool nonG0V0Server)
    {
        ApiUrl = apiUrl.RemoveSuffix("/");
        WebsiteUrl = websiteUrl.RemoveSuffix("/");
        ClientId = clientId;
        ClientSecret = clientSecret;
        SpectatorUrl = spectatorUrl.RemoveSuffix("/");
        MultiplayerUrl = multiplayerUrl.RemoveSuffix("/");
        MetadataUrl = metadataUrl.RemoveSuffix("/");
        BeatmapSubmissionServiceUrl = beatmapSubmissionServiceUrl.RemoveSuffix("/");
        DisableSentryLogger = disableSentryLogger;
        NonG0V0Server = nonG0V0Server;
    }

    public string ApiUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string SpectatorUrl { get; set; } = string.Empty;
    public string MultiplayerUrl { get; set; } = string.Empty;
    public string MetadataUrl { get; set; } = string.Empty;
    public string BeatmapSubmissionServiceUrl { get; set; } = string.Empty;
    public bool DisableSentryLogger { get; set; } = true;
    public bool NonG0V0Server { get; set; } = false;
}
