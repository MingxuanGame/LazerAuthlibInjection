using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.EnhancedAuth.Objects;
using osu.Game.Rulesets.EnhancedAuth.Objects.Drawables;
using osu.Game.Rulesets.EnhancedAuth.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.EnhancedAuth.UI
{
    [Cached]
    public partial class DrawableEnhancedAuthRuleset(
        EnhancedAuthRuleset ruleset,
        IBeatmap beatmap,
        IReadOnlyList<Mod> mods = null)
        : DrawableRuleset<EnhancedAuthHitObject>(ruleset, beatmap, mods)
    {
        protected override Playfield CreatePlayfield() => new EnhancedAuthPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) =>
            new EnhancedAuthFramedReplayInputHandler(replay);

        public override DrawableHitObject<EnhancedAuthHitObject>
            CreateDrawableRepresentation(EnhancedAuthHitObject h) => new DrawableEnhancedAuthHitObject(h);

        protected override PassThroughInputManager CreateInputManager() =>
            new EnhancedAuthInputManager(Ruleset?.RulesetInfo);
    }
}
