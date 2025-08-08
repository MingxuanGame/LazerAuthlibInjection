using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Graphics;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.AuthlibInjection.Configuration;

namespace osu.Game.Rulesets.AuthlibInjection.UI;

public partial class AuthlibSettingsSubsection(Ruleset ruleset) : RulesetSettingsSubsection(ruleset)
{
    private const int delay = 1500;
    private readonly Ruleset ruleset = ruleset;

    // ReSharper disable once InconsistentNaming
    private SettingsTextBox ApiUrl = null!;

    private AuthlibRulesetConfig authlibRulesetConfig = new();

    // ReSharper disable once InconsistentNaming
    private SettingsTextBox BeatmapSubmissionServiceUrl = null!;

    // ReSharper disable once InconsistentNaming
    private SettingsTextBox ClientId = null!;

    // ReSharper disable once InconsistentNaming
    private SettingsTextBox ClientSecret = null!;

    private string filePath = "";
    private int isInitialLoading;

    // ReSharper disable once InconsistentNaming
    private SettingsTextBox MetadataUrl = null!;

    // ReSharper disable once InconsistentNaming
    private SettingsTextBox MultiplayerUrl = null!;

    // ReSharper disable once InconsistentNaming
    private SettingsTextBox SpectatorUrl = null!;

    // ReSharper disable once InconsistentNaming
    private SettingsTextBox WebsiteUrl = null!;
    [CanBeNull] private ScheduledDelegate writeToFile;

    private AuthlibRulesetConfigManager config => (AuthlibRulesetConfigManager)Config;

    protected override LocalisableString Header => ruleset.Description;

    // [CanBeNull] [Resolved] private OsuGame game { get; set; }

    [Resolved] protected INotificationOverlay Notifications { get; private set; } = null!;


    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        filePath = storage.GetFullPath(AuthlibRulesetConfig.CONFIG_FILE_NAME);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            authlibRulesetConfig =
                JsonConvert.DeserializeObject<AuthlibRulesetConfig>(json) ?? new AuthlibRulesetConfig();
        }

        Children =
        [
            ApiUrl = new SettingsTextBox()
            {
                LabelText = "API Url",
                Current = config.GetBindable<string>(AuthlibRulesetSettings.ApiUrl)
            },
            WebsiteUrl = new SettingsTextBox()
            {
                LabelText = "Website Url",
                Current = config.GetBindable<string>(AuthlibRulesetSettings.WebsiteUrl)
            },
            ClientId = new SettingsTextBox()
            {
                LabelText = "Client ID",
                Current = config.GetBindable<string>(AuthlibRulesetSettings.ClientId)
            },
            ClientSecret = new SettingsTextBox()
            {
                LabelText = "Client Secret",
                Current = config.GetBindable<string>(AuthlibRulesetSettings.ClientSecret)
            },
            SpectatorUrl = new SettingsTextBox()
            {
                LabelText = "Spectator Url",
                Current = config.GetBindable<string>(AuthlibRulesetSettings.SpectatorUrl)
            },
            MultiplayerUrl = new SettingsTextBox()
            {
                LabelText = "Multiplayer Url",
                Current = config.GetBindable<string>(AuthlibRulesetSettings.MultiplayerUrl)
            },
            MetadataUrl = new SettingsTextBox()
            {
                LabelText = "Metadata Url",
                Current = config.GetBindable<string>(AuthlibRulesetSettings.MetadataUrl)
            },
            BeatmapSubmissionServiceUrl = new SettingsTextBox()
            {
                LabelText = "Beatmap Submission Service Url",
                Current = config.GetBindable<string>(AuthlibRulesetSettings.BeatmapSubmissionServiceUrl)
            }
        ];
        isInitialLoading = Children.Count;
        ApiUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(nameof(ApiUrl), e), true);
        WebsiteUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(nameof(WebsiteUrl), e), true);
        ClientId.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(nameof(ClientId), e), true);
        ClientSecret.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(nameof(ClientSecret), e), true);
        SpectatorUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(nameof(SpectatorUrl), e), true);
        MultiplayerUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(nameof(MultiplayerUrl), e), true);
        MetadataUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(nameof(MetadataUrl), e), true);
        BeatmapSubmissionServiceUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(nameof(BeatmapSubmissionServiceUrl), e), true);
    }

    private void onCustomApiUrlChanged(string from, ValueChangedEvent<string> e)
    {
        if (isInitialLoading > 0)
        {
            --isInitialLoading;
        }

        Logger.Log(from + "from " + e.OldValue + " changed to " + e.NewValue, target: LoggingTarget.Runtime);
        Logger.Log(isInitialLoading.ToString());
        PropertyInfo props = authlibRulesetConfig.GetType().GetProperty(from);
        if (props != null)
        {
            string value = (string)props.GetValue(authlibRulesetConfig);
            if (string.Equals(value, e.NewValue, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            props.SetValue(authlibRulesetConfig, e.NewValue);
        }

        writeToFile?.Cancel();
        writeToFile = Scheduler.AddDelayed(() =>
        {
            File.WriteAllText(
                filePath,
                JsonConvert.SerializeObject(authlibRulesetConfig)
            );
            Notifications.Post(new ApiChangedNotification());
        }, delay);
    }

    private partial class ApiChangedNotification : SimpleNotification
    {
        public ApiChangedNotification()
        {
            Text = "API settings changed, please restart the game to apply changes.";
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Icon = FontAwesome.Solid.Server;
            IconContent.Colour = colours.BlueDark;
        }
    }
}
