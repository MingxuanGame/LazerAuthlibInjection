using HarmonyLib;
using osu.Game.Online;

namespace osu.Game.Rulesets.AuthlibInjection.Patches;

[HarmonyPatch(typeof(TrustedDomainOnlineStore), "GetLookupUrl")]
public class TrustedDomainPatch
{
    static bool Prefix(ref string __result, string url)
    {
        __result = url;
        return false;
    }
}
