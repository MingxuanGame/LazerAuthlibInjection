using HarmonyLib;
using osu.Game.Extensions;

namespace osu.Game.Rulesets.AuthlibInjection.Patches;

[HarmonyPatch(typeof(ModelExtensions), nameof(ModelExtensions.IsLegacyRuleset))]
[HarmonyPriority(Priority.High)]
public class ModelExtensionsPatch
{
    static bool Prefix(IRulesetInfo ruleset, ref bool __result)
    {
        __result = ruleset.OnlineID >= 0;
        return false;
    }
}
