using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;

namespace InfiniteInventory.patches;

[HarmonyPatch(typeof(ItemStorage), nameof(ItemStorage.getItemStockable))]
public class ItemStorage_getItemStockable
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_0), new CodeMatch(OpCodes.Brtrue));
        codeMatcher.Advance(1);
        codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1), Transpilers.EmitDelegate(PushPredicate));
        return codeMatcher.InstructionEnumeration();
    }

    static bool PushPredicate(bool flag, NelItem Itm)
    {
        return !(!flag && (Itm.isEmptyBottle() || Itm.isEmptyLunchBox()));
    }
}