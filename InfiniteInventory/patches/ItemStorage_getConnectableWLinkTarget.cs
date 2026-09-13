using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;

namespace InfiniteInventory.patches;

[HarmonyPatch(typeof(ItemStorage), "getConnectableWLinkTarget")]
public class ItemStorage_getConnectableWLinkTarget
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Ldfld), new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Ldfld), new CodeMatch(OpCodes.Blt));
        if (!codeMatcher.IsValid)
        {
            Plugin.Logger.LogError("IL matching failure in ItemStorage_getConnectableWLinkTarget transpiler.");
            return codeMatcher.InstructionEnumeration();
        }
        var label = codeMatcher.InstructionAt(4).operand;
        codeMatcher.RemoveInstructions(5);
        codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Br, label));
        return codeMatcher.InstructionEnumeration();
    }
}