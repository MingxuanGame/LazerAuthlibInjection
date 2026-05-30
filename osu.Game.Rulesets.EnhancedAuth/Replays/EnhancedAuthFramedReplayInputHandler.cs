using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.EnhancedAuth.Replays
{
    public class EnhancedAuthFramedReplayInputHandler(Replay replay)
        : FramedReplayInputHandler<EnhancedAuthReplayFrame>(replay)
    {
        protected override bool IsImportant(EnhancedAuthReplayFrame frame) => frame.Actions.Any();

        protected override void CollectReplayInputs(List<IInput> inputs)
        {
        }
    }
}
