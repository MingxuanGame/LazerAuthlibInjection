using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.EnhancedAuth
{
    public class EnhancedAuthDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
        : DifficultyCalculator(ruleset, beatmap)
    {
        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills)
        {
            return new DifficultyAttributes(mods, 0);
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap,
                                                                                       Mod[] mods) => [];

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods) => [];
    }
}
