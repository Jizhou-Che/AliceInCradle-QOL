using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;

namespace InfiniteInventory.patches;

[HarmonyPatch(typeof(ItemStorage), nameof(ItemStorage.Add))]
[HarmonyPatch([typeof(NelItem), typeof(int), typeof(int), typeof(ItemStorage.IRow), typeof(bool), typeof(bool)], [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal])]
public class ItemStorage_Add
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_0), new CodeMatch(OpCodes.Blt));
        codeMatcher.RemoveInstructions(3);
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Brfalse));
        codeMatcher.RemoveInstructions(2);
        return codeMatcher.InstructionEnumeration();
    }
}