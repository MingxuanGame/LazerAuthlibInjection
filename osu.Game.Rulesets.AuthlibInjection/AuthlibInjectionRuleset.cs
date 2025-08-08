﻿using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.AuthlibInjection.Beatmaps;
using osu.Game.Rulesets.AuthlibInjection.Configuration;
using osu.Game.Rulesets.AuthlibInjection.UI;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.AuthlibInjection
{
    public partial class AuthlibInjectionRuleset : Ruleset
    {
        private const string short_name = "authlibinjectionruleset";

        public AuthlibInjectionRuleset()
        {
            var harmony = new Harmony(short_name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string Description => "Custom server support for osu!lazer";

        public override string ShortName => short_name;

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) =>
            new DrawableAuthlibInjectionRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) =>
            new AuthlibInjectionBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) =>
            new AuthlibInjectionDifficultyCalculator(RulesetInfo, beatmap);

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) =>
            new AuthlibRulesetConfigManager(settings, RulesetInfo);

        public override RulesetSettingsSubsection CreateSettings() => new AuthlibSettingsSubsection(this);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            return type switch
            {
                _ => Array.Empty<Mod>()
            };
        }

        public override Drawable CreateIcon() => new InjectorIcon();

        public partial class InjectorIcon : CompositeDrawable
        {
            public InjectorIcon()
            {
                Size = new Vector2(20);
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
                        Size = new Vector2(0.6f),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Solid.Hammer,
                        RelativeSizeAxes = Axes.Both,
                    }
                ];
            }
        }
    }
}
