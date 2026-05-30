#nullable enable
using System;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Configuration;
using osu.Game.Rulesets.EnhancedAuth.Extensions;

namespace osu.Game.Rulesets.EnhancedAuth.Configuration;

public class GlobalConfigManager
{
    private const string osu_prod_api_url = "https://osu.ppy.sh";
    private const string osu_dev_api_url = "https://dev.ppy.sh";

    private static readonly object @lock = new();
    private static EnhancedRulesetConfig? instance;
    private static RulesetHashCache? hashCache;
    private static OsuGameBase? gameBase;

    public static OsuGameBase GameBase =>
        gameBase ?? throw new InvalidOperationException("GameBase is not initialized.");

    public static EnhancedRulesetConfig Config =>
        instance ?? throw new InvalidOperationException("EnhancedRulesetConfig is not initialized.");

    public static RulesetHashCache HashCache =>
        hashCache ?? throw new InvalidOperationException("HashCache is not initialized.");

    public static bool IsOfficialServer(string apiUrl)
    {
        return apiUrl is osu_prod_api_url or osu_dev_api_url;
    }

    public static bool Patched => instance != null && !string.IsNullOrEmpty(instance.ApiUrl) && !IsOfficialServer(instance.ApiUrl);

    private static EnhancedRulesetConfig readFromCommandLine(EnhancedRulesetConfig? config)
    {
        config ??= new EnhancedRulesetConfig();
        string[] args = Environment.GetCommandLineArgs();

        foreach (string arg in args)
        {
            string[] split = arg.Split('=');

            string key = split[0];
            string val = split.Length > 1 ? split[1] : string.Empty;

            if (string.IsNullOrEmpty(val))
            {
                switch (key)
                {
                    case "--disable-sentry-logger":
                        config.DisableSentryLogger = true;
                        continue;

                    case "--non-g0v0-server":
                        config.NonG0V0Server = true;
                        continue;

                    default:
                        continue;
                }
            }

            switch (key)
            {
                case "--api-url":
                case "-devserver": // stable like
                    config.ApiUrl = val.AddHttpsProtocol();
                    break;

                case "--website-url":
                    config.WebsiteUrl = val.AddHttpsProtocol();
                    break;

                case "--client-id":
                    config.ClientId = val;
                    break;

                case "--client-secret":
                    config.ClientSecret = val;
                    break;

                case "--spectator-url":
                    config.SpectatorUrl = val.AddHttpsProtocol();
                    break;

                case "--multiplayer-url":
                    config.MultiplayerUrl = val.AddHttpsProtocol();
                    break;

                case "--metadata-url":
                    config.MetadataUrl = val.AddHttpsProtocol();
                    break;

                case "--bss-url":
                    config.BeatmapSubmissionServiceUrl = val.AddHttpsProtocol();
                    break;
            }
        }

        return config;
    }

    private static EnhancedRulesetConfig? readFromFile(string configPath)
    {
        if (!File.Exists(configPath))
        {
            Logger.Log("[EnhancedAuth] authlib_local_config.json not found, using default config.",
                level: LogLevel.Verbose);
            return null;
        }

        string config = File.ReadAllText(configPath);

        if (string.IsNullOrEmpty(config))
        {
            Logger.Log("[EnhancedAuth] authlib_local_config.json is empty, please check the file.",
                level: LogLevel.Verbose);
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<EnhancedRulesetConfig>(config);
        }
        catch (JsonException)
        {
            Logger.Log(
                "[EnhancedAuth] Failed to parse authlib_local_config.json, please check the json format.",
                level: LogLevel.Error);
        }

        return null;
    }

    public static void InitializeGameBase(OsuGameBase osuGameBase)
    {
        lock (@lock)
        {
            gameBase ??= osuGameBase;
        }
    }

    public static void InitializeConfig()
    {
        lock (@lock)
        {
            // try get game folder
            var localConfig = Traverse.Create(GameBase).Property("LocalConfig").GetValue<OsuConfigManager>();
            string configPath = EnhancedRulesetConfig.CONFIG_FILE_NAME;

            if (localConfig != null)
            {
                var storage = Traverse.Create(localConfig).Field("storage").GetValue<Storage>();
                configPath = storage.GetFullPath(EnhancedRulesetConfig.CONFIG_FILE_NAME);
            }

            EnhancedRulesetConfig? authlibLocalConfig = readFromFile(configPath);

            instance = readFromCommandLine(authlibLocalConfig);
        }
    }

    public static void InitializeHashCache(RulesetStore store)
    {
        lock (@lock)
        {
            hashCache ??= new RulesetHashCache(store);
        }
    }

    public static void InitializeHashCache()
    {
        // try get ruleset store
        var store = Traverse.Create(GameBase).Property("RulesetStore").GetValue<RulesetStore>();
        InitializeHashCache(store);
    }
}
