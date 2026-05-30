using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.EnhancedAuth.Beatmaps;
using osu.Game.Rulesets.EnhancedAuth.Configuration;
using osu.Game.Rulesets.EnhancedAuth.Notifications;
using osu.Game.Rulesets.EnhancedAuth.Patches;
using osu.Game.Rulesets.EnhancedAuth.UI;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.EnhancedAuth
{
    public partial class EnhancedAuthRuleset : Ruleset
    {
        private const string short_name = "enhancedauthruleset";

        private static bool currentServerNotificationPosted;

        public EnhancedAuthRuleset()
        {
            var harmony = new Harmony(short_name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string Description => "Custom server support for osu!lazer";

        public override string ShortName => short_name;

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) =>
            new DrawableEnhancedAuthRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) =>
            new EnhancedAuthBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) =>
            new EnhancedAuthDifficultyCalculator(RulesetInfo, beatmap);

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) =>
            new EnhancedRulesetConfigManager(settings, RulesetInfo);

        public override RulesetSettingsSubsection CreateSettings() => new EnhancedSettingsSubsection(this);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            return type switch
            {
                _ => Array.Empty<Mod>()
            };
        }

        public override Drawable CreateIcon() => new EnhancedAuthIcon();

        public partial class EnhancedAuthIcon : CompositeDrawable
        {
            public EnhancedAuthIcon()
            {
                AutoSizeAxes = Axes.Both;
                InternalChildren =
                [
                    new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Regular.Circle,
                    },
                    new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Scale = new Vector2(0.5f),
                        Icon = FontAwesome.Solid.Hammer,
                    }
                ];
            }

            [BackgroundDependencyLoader]
            private void load(OsuGame game, INotificationOverlay notifications)
            {
                if (GlobalConfigManager.Config.DisableSentryLogger)
                    DisableSentryPatch.Run(game);

                if (currentServerNotificationPosted) return;

                notifications.Post(new CurrentServerNotification());
                currentServerNotificationPosted = true;
            }
        }
    }
}
