using HarmonyLib;
using nel;

namespace Demosaic.patches;

[HarmonyPatch(typeof(MosaicShower), "FnDrawMosaic")]
public class MosaicShower_FnDrawMosaic
{
    static void Postfix(ref bool __result)
    {
        __result = false;
    }
}