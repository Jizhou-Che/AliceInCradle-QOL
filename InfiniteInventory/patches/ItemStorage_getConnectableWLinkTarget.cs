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
        var label = codeMatcher.InstructionAt(4).operand;
        codeMatcher.RemoveInstructions(5);
        codeMatcher.Insert(new CodeInstruction(OpCodes.Br, label));
        return codeMatcher.InstructionEnumeration();
    }
}