using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Graphics;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.AuthlibInjection.Configuration;
using osu.Game.Rulesets.AuthlibInjection.Patches;

namespace osu.Game.Rulesets.AuthlibInjection.UI;

public partial class AuthlibSettingsSubsection(Ruleset ruleset) : RulesetSettingsSubsection(ruleset)
{
    private const int delay = 1500;
    private readonly Ruleset ruleset = ruleset;
    private AuthlibRulesetConfig authlibRulesetConfig = new();

    // Considered for distinction, batch disabling
    // ReSharper disable InconsistentNaming
    private SettingsCheckbox DisableSentryLogging = null!;

    private SettingsTextBox ApiUrl = null!;

    private SettingsTextBox BeatmapSubmissionServiceUrl = null!;

    private SettingsTextBox ClientId = null!;

    private SettingsTextBox ClientSecret = null!;

    private SettingsTextBox MetadataUrl = null!;

    private SettingsTextBox MultiplayerUrl = null!;

    private SettingsTextBox SpectatorUrl = null!;

    private SettingsTextBox WebsiteUrl = null!;
    // ReSharper restore InconsistentNaming

    private string filePath = "";
    private int isInitialLoading;

    [CanBeNull] private ScheduledDelegate writeToFile;

    private AuthlibRulesetConfigManager config => (AuthlibRulesetConfigManager)Config;

    protected override LocalisableString Header => ruleset.Description;

    // [CanBeNull] [Resolved] private OsuGame game { get; set; }

    [Resolved] protected INotificationOverlay Notifications { get; private set; } = null!;


    [BackgroundDependencyLoader]
    private void load(OsuGame game, Storage storage)
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
            },
            DisableSentryLogging = new SettingsCheckbox()
            {
                LabelText = "Disable Sentry Logger",
                TooltipText = "Stop sending telemetry error data to the osu! dev team.",
                Current = config.GetBindable<bool>(AuthlibRulesetSettings.DisableSentryLogger)
            }
        ];
        isInitialLoading = Children.Count;
        ApiUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(ApiUrl, nameof(ApiUrl), e), true);
        WebsiteUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(WebsiteUrl, nameof(WebsiteUrl), e), true);
        ClientId.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(ClientId, nameof(ClientId), e), true);
        ClientSecret.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(ClientSecret, nameof(ClientSecret), e), true);
        SpectatorUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(SpectatorUrl, nameof(SpectatorUrl), e), true);
        MultiplayerUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(MultiplayerUrl, nameof(MultiplayerUrl), e), true);
        MetadataUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(MetadataUrl, nameof(MetadataUrl), e), true);
        BeatmapSubmissionServiceUrl.Current.BindValueChanged(e =>
            onCustomApiUrlChanged(BeatmapSubmissionServiceUrl, nameof(BeatmapSubmissionServiceUrl), e), true);
        DisableSentryLogging.Current.BindValueChanged(e => onSentryOptOutChanged(e, game), true);
    }

    private void onSentryOptOutChanged(ValueChangedEvent<bool> e, OsuGame game)
    {
        File.WriteAllText(filePath, JsonConvert.SerializeObject(authlibRulesetConfig));

        // When switching from off to on, try to disable potentially active logger instance.
        if (e.NewValue)
        {
            DisableSentryPatch.Run(game);
        }
    }

    private void onCustomApiUrlChanged(SettingsTextBox input, string from, ValueChangedEvent<string> e)
    {
        if (isInitialLoading > 0)
        {
            --isInitialLoading;
        }

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
            bool removedSuffix = false;
            if (from.EndsWith("Url") && e.NewValue.EndsWith("/"))
            {
                input.Current.Value = e.NewValue.TrimEnd('/');
                removedSuffix = true;
            }

            File.WriteAllText(
                filePath,
                JsonConvert.SerializeObject(authlibRulesetConfig)
            );
            if (!removedSuffix)
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
