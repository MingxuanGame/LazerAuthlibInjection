using System.IO;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.EnhancedAuth.Configuration;
using osu.Game.Rulesets.EnhancedAuth.Extensions;
using osu.Game.Rulesets.EnhancedAuth.Notifications;
using osu.Game.Rulesets.EnhancedAuth.Patches;
using osuTK;

namespace osu.Game.Rulesets.EnhancedAuth.UI;

public partial class EnhancedSettingsSubsection(Ruleset ruleset) : RulesetSettingsSubsection(ruleset)
{
    private const int delay = 1500;
    private readonly Ruleset ruleset = ruleset;
    private EnhancedRulesetConfig authlibRulesetConfig = new EnhancedRulesetConfig();

    private FormCheckBox disableSentryLogging = null!;
    private FormCheckBox nonG0V0Server = null!;

    private FormTextBox apiUrl = null!;
    private FormTextBox beatmapSubmissionServiceUrl = null!;

    private FormTextBox clientId = null!;
    private FormTextBox clientSecret = null!;

    private FormTextBox metadataUrl = null!;
    private FormTextBox multiplayerUrl = null!;
    private FormTextBox spectatorUrl = null!;
    private FormTextBox websiteUrl = null!;

    private FillFlowContainer advancedSettingsFlow = null!;

    private string filePath = "";

    private readonly BindableBool showAdvancedSettings = new BindableBool();
    private readonly Bindable<SettingsNote.Data> advancedSettingsWarning = new Bindable<SettingsNote.Data>();

    private EnhancedRulesetConfigManager config => (EnhancedRulesetConfigManager)Config;

    protected override LocalisableString Header => ruleset.Description;

    // [CanBeNull] [Resolved] private OsuGame game { get; set; }

    [Resolved]
    protected INotificationOverlay Notifications { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load(OsuGame game, Storage storage)
    {
        // Added for smooth transition of advanced settings section
        AutoSizeDuration = 300;
        AutoSizeEasing = Easing.OutQuint;

        filePath = storage.GetFullPath(EnhancedRulesetConfig.CONFIG_FILE_NAME);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            authlibRulesetConfig =
                JsonConvert.DeserializeObject<EnhancedRulesetConfig>(json) ?? new EnhancedRulesetConfig();
        }

        Children =
        [
            new SettingsItemV2(apiUrl = new FormTextBox
            {
                Caption = "API Url",
                Current = config.GetBindable<string>(EnhancedRulesetSettings.ApiUrl),
            }),
            new SettingsItemV2(websiteUrl = new FormTextBox
            {
                Caption = "Website Url",
                Current = config.GetBindable<string>(EnhancedRulesetSettings.WebsiteUrl),
            }),
            new SettingsItemV2(new FormCheckBox
            {
                Caption = "Show Advanced",
                Current = showAdvancedSettings,
            })
            {
                Note =
                {
                    BindTarget = advancedSettingsWarning,
                },
            },
            advancedSettingsFlow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(5),
                Alpha = 0,
                Children =
                [
                    new SettingsItemV2(clientId = new FormTextBox
                    {
                        Caption = "Client ID",
                        Current = config.GetBindable<string>(EnhancedRulesetSettings.ClientId),
                    }),
                    new SettingsItemV2(clientSecret = new FormTextBox
                    {
                        Caption = "Client Secret",
                        Current = config.GetBindable<string>(EnhancedRulesetSettings.ClientSecret),
                    }),
                    new SettingsItemV2(spectatorUrl = new FormTextBox
                    {
                        Caption = "Spectator Url",
                        Current = config.GetBindable<string>(EnhancedRulesetSettings.SpectatorUrl),
                    }),
                    new SettingsItemV2(multiplayerUrl = new FormTextBox
                    {
                        Caption = "Multiplayer Url",
                        Current = config.GetBindable<string>(EnhancedRulesetSettings.MultiplayerUrl)
                    }),
                    new SettingsItemV2(metadataUrl = new FormTextBox
                    {
                        Caption = "Metadata Url",
                        Current = config.GetBindable<string>(EnhancedRulesetSettings.MetadataUrl),
                    }),
                    new SettingsItemV2(beatmapSubmissionServiceUrl = new FormTextBox
                    {
                        Caption = "Beatmap Submission Service Url",
                        Current = config.GetBindable<string>(EnhancedRulesetSettings.BeatmapSubmissionServiceUrl),
                    }),
                    new SettingsItemV2(nonG0V0Server = new FormCheckBox
                    {
                        Caption = "Is non-g0v0-server",
                        HintText = "Whether the server is a GooGuTeam/g0v0-server instance. You can view https://<api-url>/docs to identify",
                        Current = config.GetBindable<bool>(EnhancedRulesetSettings.NonG0V0Server),
                    }),
                    new SettingsItemV2(disableSentryLogging = new FormCheckBox
                    {
                        Caption = "Disable Sentry Logger",
                        HintText = "Stop sending telemetry error data to the osu! dev team.",
                        Current = config.GetBindable<bool>(EnhancedRulesetSettings.DisableSentryLogger),
                    }),
                ],
            },
            new SettingsButtonV2
            {
                Text = "Save Changes",
                Action = onSaveChanges,
            },
        ];

        showAdvancedSettings.BindValueChanged(onAdvancedVisibilityChanged);
        disableSentryLogging.Current.BindValueChanged(e => onSentryOptOutChanged(e, game), true);
    }

    private void onAdvancedVisibilityChanged(ValueChangedEvent<bool> e)
    {
        advancedSettingsFlow.FadeTo(e.NewValue ? 1 : 0, 300, Easing.OutQuint);
        advancedSettingsWarning.Value = e.NewValue ? new SettingsNote.Data("These settings are not intended for normal gameplay. Use with caution!", SettingsNote.Type.Warning) : null;
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

    private void onSaveChanges()
    {
        foreach (var textBox in (FormTextBox[])[apiUrl, websiteUrl, spectatorUrl, multiplayerUrl, metadataUrl, beatmapSubmissionServiceUrl])
        {
            if (!string.IsNullOrEmpty(textBox.Current.Value))
                textBox.Current.Value = textBox.Current.Value.RemoveSuffix("/").AddHttpsProtocol();
        }

        authlibRulesetConfig = new EnhancedRulesetConfig
        {
            ApiUrl = apiUrl.Current.Value,
            WebsiteUrl = websiteUrl.Current.Value,
            ClientId = clientId.Current.Value,
            ClientSecret = clientSecret.Current.Value,
            SpectatorUrl = spectatorUrl.Current.Value,
            MultiplayerUrl = multiplayerUrl.Current.Value,
            MetadataUrl = metadataUrl.Current.Value,
            BeatmapSubmissionServiceUrl = beatmapSubmissionServiceUrl.Current.Value,
            DisableSentryLogger = disableSentryLogging.Current.Value,
            NonG0V0Server = nonG0V0Server.Current.Value,
        };

        File.WriteAllText(
            filePath,
            JsonConvert.SerializeObject(authlibRulesetConfig)
        );
        Notifications.Post(new ApiChangedNotification());
    }
}
