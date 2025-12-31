using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;

namespace AutomaticReels.patches;

[HarmonyPatch(typeof(ReelExecuter), nameof(ReelExecuter.applyEffectToIK))]
public class ReelExecuter_applyEffectToIK
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)99));
        codeMatcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4, 9999));
        return codeMatcher.InstructionEnumeration();
    }
}