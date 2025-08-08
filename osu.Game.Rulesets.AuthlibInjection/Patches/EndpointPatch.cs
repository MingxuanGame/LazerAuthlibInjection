using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using osu.Framework.Platform;
using osu.Game.Configuration;
using osu.Game.Online;
using osu.Game.Rulesets.AuthlibInjection.Configuration;

namespace osu.Game.Rulesets.AuthlibInjection.Patches;

[HarmonyPatch(typeof(OsuGameBase), nameof(OsuGameBase.CreateEndpoints))]
public class EndpointPatch
{
    static void Postfix(OsuGameBase __instance, ref EndpointConfiguration __result)
    {
        // try get game folder
        var localConfig = Traverse.Create(__instance).Property("LocalConfig").GetValue<OsuConfigManager>();
        string configPath = AuthlibRulesetConfig.CONFIG_FILE_NAME;
        if (localConfig != null)
        {
            var storage = Traverse.Create(localConfig).Field("storage").GetValue<Storage>();
            configPath = storage.GetFullPath(AuthlibRulesetConfig.CONFIG_FILE_NAME);
        }

        if (File.Exists(configPath) == false)
        {
            return;
        }

        string config = File.ReadAllText(configPath);
        if (string.IsNullOrEmpty(config))
        {
            return;
        }

        var authlibLocalConfig = JsonConvert.DeserializeObject<AuthlibRulesetConfig>(config);

        if (!string.IsNullOrEmpty(authlibLocalConfig.ApiUrl))
        {
            __result.APIUrl = authlibLocalConfig.ApiUrl;
        }

        if (!string.IsNullOrEmpty(authlibLocalConfig.WebsiteUrl))
        {
            __result.WebsiteUrl = authlibLocalConfig.WebsiteUrl;
        }
        else if (!string.IsNullOrEmpty(authlibLocalConfig.ApiUrl))
        {
            __result.WebsiteUrl = authlibLocalConfig.ApiUrl;
        }

        if (!string.IsNullOrEmpty(authlibLocalConfig.SpectatorUrl))
        {
            __result.SpectatorUrl = authlibLocalConfig.SpectatorUrl;
        }
        else if (!string.IsNullOrEmpty(authlibLocalConfig.ApiUrl))
        {
            __result.SpectatorUrl = __result.APIUrl + "/signalr/spectator";
        }

        if (!string.IsNullOrEmpty(authlibLocalConfig.MultiplayerUrl))
        {
            __result.MultiplayerUrl = authlibLocalConfig.MultiplayerUrl;
        }
        else if (!string.IsNullOrEmpty(authlibLocalConfig.ApiUrl))
        {
            __result.MultiplayerUrl = __result.APIUrl + "/signalr/multiplayer";
        }

        if (!string.IsNullOrEmpty(authlibLocalConfig.MetadataUrl))
        {
            __result.MetadataUrl = authlibLocalConfig.MetadataUrl;
        }
        else if (!string.IsNullOrEmpty(authlibLocalConfig.ApiUrl))
        {
            __result.MetadataUrl = __result.APIUrl + "/signalr/metadata";
        }

        if (!string.IsNullOrEmpty(authlibLocalConfig.BeatmapSubmissionServiceUrl))
        {
            __result.BeatmapSubmissionServiceUrl = authlibLocalConfig.BeatmapSubmissionServiceUrl;
        }
        else if (!string.IsNullOrEmpty(authlibLocalConfig.ApiUrl))
        {
            __result.BeatmapSubmissionServiceUrl = __result.APIUrl + "/beatmap-submission";
        }

        if (!string.IsNullOrEmpty(authlibLocalConfig.ClientId))
        {
            __result.APIClientID = authlibLocalConfig.ClientId;
        }

        if (!string.IsNullOrEmpty(authlibLocalConfig.ClientSecret))
        {
            __result.APIClientSecret = authlibLocalConfig.ClientSecret;
        }
    }
}
