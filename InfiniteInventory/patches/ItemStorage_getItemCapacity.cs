using HarmonyLib;
using nel;

namespace InfiniteInventory.patches;

[HarmonyPatch(typeof(ItemStorage), nameof(ItemStorage.getItemCapacity))]
public class ItemStorage_getItemCapacity
{
    static void Postfix(ref int __result)
    {
        __result = 9999;
    }
}