using osu.Game.Beatmaps;
using osu.Game.Rulesets.EnhancedAuth.Objects;

namespace osu.Game.Rulesets.EnhancedAuth.Beatmaps
{
    public class EnhancedAuthBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
        : BeatmapConverter<EnhancedAuthHitObject>(beatmap, ruleset)
    {
        public override bool CanConvert() => false;
    }
}
