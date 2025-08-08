namespace osu.Game.Rulesets.AuthlibInjection.Configuration;

public class AuthlibRulesetConfig
{
    public const string CONFIG_FILE_NAME = "authlib_local_config.json";
    public AuthlibRulesetConfig() { }

    public AuthlibRulesetConfig(string apiUrl,
        string websiteUrl,
        string clientId,
        string clientSecret,
        string spectatorUrl,
        string multiplayerUrl,
        string metadataUrl,
        string beatmapSubmissionServiceUrl)
    {
        ApiUrl = removeSuffix(apiUrl, "/");
        WebsiteUrl = removeSuffix(websiteUrl, "/");
        ClientId = clientId;
        ClientSecret = clientSecret;
        SpectatorUrl = removeSuffix(spectatorUrl, "/");
        MultiplayerUrl = removeSuffix(multiplayerUrl, "/");
        MetadataUrl = removeSuffix(metadataUrl, "/");
        BeatmapSubmissionServiceUrl = removeSuffix(beatmapSubmissionServiceUrl, "/");
    }

    public string ApiUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string SpectatorUrl { get; set; } = string.Empty;
    public string MultiplayerUrl { get; set; } = string.Empty;
    public string MetadataUrl { get; set; } = string.Empty;
    public string BeatmapSubmissionServiceUrl { get; set; } = string.Empty;

    private static string removeSuffix(string text, string suffix)
    {
        if (text.EndsWith(suffix))
        {
            return text.Substring(0, text.Length - suffix.Length);
        }

        return text;
    }
}
