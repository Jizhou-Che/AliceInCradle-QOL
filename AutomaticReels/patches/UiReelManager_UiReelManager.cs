using HarmonyLib;
using nel;

namespace AutomaticReels.patches;

[HarmonyPatch(typeof(UiReelManager), MethodType.Constructor)]
public class UiReelManager_UiReelManager
{
    static void Postfix(UiReelManager __instance)
    {
        __instance.autodecide_progressable = false;
    }
}