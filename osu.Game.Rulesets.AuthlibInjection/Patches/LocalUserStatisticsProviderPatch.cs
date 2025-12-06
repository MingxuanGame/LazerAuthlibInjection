using System.Linq;
using HarmonyLib;
using osu.Game.Extensions;
using osu.Game.Online;
using osu.Game.Rulesets.AuthlibInjection.Configuration;
using osu.Game.Rulesets.AuthlibInjection.Extensions;

namespace osu.Game.Rulesets.AuthlibInjection.Patches;

[HarmonyPatch(typeof(LocalUserStatisticsProvider), "initialiseStatistics")]
public class LocalUserStatisticsProviderPatch
{
    static void Postfix(LocalUserStatisticsProvider __instance)
    {
        if (!GlobalConfigManager.Patched)
        {
            return;
        }

        var rulesets = Traverse.Create(__instance).Property("rulesets").GetValue<RulesetStore>();

        foreach (var ruleset in rulesets.AvailableRulesets.Where(r => r.IsLegacyRuleset()))
        {
            switch (ruleset.ShortName)
            {
                case RulesetInfoExtension.OSU_MODE_SHORTNAME:
                    __instance.RefetchStatistics(ruleset.CreateSpecialRuleset(RulesetInfoExtension.OSU_RELAX_MODE_SHORTNAME, RulesetInfoExtension.OSU_RELAX_ONLINE_ID));
                    __instance.RefetchStatistics(ruleset.CreateSpecialRuleset(RulesetInfoExtension.OSU_AUTOPILOT_MODE_SHORTNAME, RulesetInfoExtension.OSU_AUTOPILOT_ONLINE_ID));
                    break;

                case RulesetInfoExtension.TAIKO_MODE_SHORTNAME:
                    __instance.RefetchStatistics(ruleset.CreateSpecialRuleset(RulesetInfoExtension.TAIKO_RELAX_MODE_SHORTNAME, RulesetInfoExtension.TAIKO_RELAX_ONLINE_ID));
                    break;

                case RulesetInfoExtension.CATCH_MODE_SHORTNAME:
                    __instance.RefetchStatistics(ruleset.CreateSpecialRuleset(RulesetInfoExtension.CATCH_MODE_SHORTNAME, RulesetInfoExtension.CATCH_RELAX_ONLINE_ID));
                    break;
            }
        }
    }
}
