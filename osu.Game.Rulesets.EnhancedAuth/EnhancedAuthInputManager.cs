using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.EnhancedAuth
{
    public partial class EnhancedAuthInputManager(RulesetInfo ruleset)
        : RulesetInputManager<EnhancedAuthAction>(ruleset, 0, SimultaneousBindingMode.Unique);

    public enum EnhancedAuthAction;
}
