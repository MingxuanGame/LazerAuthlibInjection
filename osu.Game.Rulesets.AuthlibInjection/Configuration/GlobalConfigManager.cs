#nullable enable
using System;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Configuration;

namespace osu.Game.Rulesets.AuthlibInjection.Configuration;

public class GlobalConfigManager
{
    private static readonly object @lock = new();
    private static AuthlibRulesetConfig? instance;

    public static AuthlibRulesetConfig Instance =>
        instance ?? throw new InvalidOperationException("AuthlibRulesetConfig is not initialized.");


    public static bool Patched => instance != null && !string.IsNullOrEmpty(instance.ApiUrl);

    private static AuthlibRulesetConfig readFromCommandLine(AuthlibRulesetConfig? config)
    {
        config ??= new AuthlibRulesetConfig();
        string[] args = Environment.GetCommandLineArgs();
        foreach (string arg in args)
        {
            string[] split = arg.Split('=');

            string key = split[0];
            string val = split.Length > 1 ? split[1] : string.Empty;
            if (string.IsNullOrEmpty(val))
            {
                continue;
            }

            if (!val.StartsWith("http://") && !val.StartsWith("https://"))
            {
                val = $"https://{val}";
            }

            switch (key)
            {
                case "--api-url":
                case "-devserver": // stable like
                    config.ApiUrl = val;
                    break;
                case "--website-url":
                    config.WebsiteUrl = val;
                    break;
                case "--client-id":
                    config.ClientId = val;
                    break;
                case "--client-secret":
                    config.ClientSecret = val;
                    break;
                case "--spectator-url":
                    config.SpectatorUrl = val;
                    break;
                case "--multiplayer-url":
                    config.MultiplayerUrl = val;
                    break;
                case "--metadata-url":
                    config.MetadataUrl = val;
                    break;
                case "--bss-url":
                    config.BeatmapSubmissionServiceUrl = val;
                    break;
            }
        }

        return config;
    }

    private static AuthlibRulesetConfig? readFromFile(string configPath)
    {
        if (!File.Exists(configPath))
        {
            Logger.Log("[AuthlibInjection] authlib_local_config.json not found, using default config.",
                level: LogLevel.Verbose);
            return null;
        }

        string config = File.ReadAllText(configPath);
        if (string.IsNullOrEmpty(config))
        {
            Logger.Log("[AuthlibInjection] authlib_local_config.json is empty, please check the file.",
                level: LogLevel.Verbose);
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<AuthlibRulesetConfig>(config);
        }
        catch (JsonException)
        {
            Logger.Log(
                "[AuthlibInjection] Failed to parse authlib_local_config.json, please check the json format.",
                level: LogLevel.Error);
        }


        return null;
    }

    public static void Initialize(OsuGameBase gameBase)
    {
        // try get game folder
        var localConfig = Traverse.Create(gameBase).Property("LocalConfig").GetValue<OsuConfigManager>();
        string configPath = AuthlibRulesetConfig.CONFIG_FILE_NAME;
        if (localConfig != null)
        {
            var storage = Traverse.Create(localConfig).Field("storage").GetValue<Storage>();
            configPath = storage.GetFullPath(AuthlibRulesetConfig.CONFIG_FILE_NAME);
        }

        AuthlibRulesetConfig? authlibLocalConfig = readFromFile(configPath);

        instance = readFromCommandLine(authlibLocalConfig);
    }
}
