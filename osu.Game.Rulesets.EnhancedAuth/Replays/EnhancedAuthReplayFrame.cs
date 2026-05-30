using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Replays;
using osuTK;

namespace osu.Game.Rulesets.EnhancedAuth.Replays
{
    public class EnhancedAuthReplayFrame : ReplayFrame
    {
        public List<EnhancedAuthAction> Actions = [];
        public Vector2 Position;

        public EnhancedAuthReplayFrame(EnhancedAuthAction? button = null)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }

        public override bool IsEquivalentTo(ReplayFrame other)
            => other is EnhancedAuthReplayFrame freeformFrame && Time == freeformFrame.Time &&
               Position == freeformFrame.Position && Actions.SequenceEqual(freeformFrame.Actions);
    }
}
