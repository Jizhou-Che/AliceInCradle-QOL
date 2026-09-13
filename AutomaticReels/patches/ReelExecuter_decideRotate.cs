using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using nel;
using UnityEngine;

namespace AutomaticReels.patches;

[HarmonyPatch(typeof(ReelExecuter), nameof(ReelExecuter.decideRotate))]
public class ReelExecuter_decideRotate
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);
        codeMatcher.MatchForward(false, new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "content_id"));
        if (!codeMatcher.IsValid)
        {
            Plugin.Logger.LogError("IL matching failure in ReelExecuter_decideRotate transpiler.");
            return codeMatcher.InstructionEnumeration();
        }
        codeMatcher.Advance(2);
        codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0), Transpilers.EmitDelegate(PushContentIdDec));
        return codeMatcher.InstructionEnumeration();
    }

    static int PushContentIdDec(int original, ReelExecuter instance)
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            return instance.getEType() switch
            {
                ReelExecuter.ETYPE.GRADE1 => 3,
                ReelExecuter.ETYPE.GRADE2 => 2,
                ReelExecuter.ETYPE.GRADE3 => 0,
                ReelExecuter.ETYPE.COUNT_ADD1 => 1,
                ReelExecuter.ETYPE.COUNT_ADD2 => 10,
                ReelExecuter.ETYPE.COUNT_ADD3 => 1,
                ReelExecuter.ETYPE.COUNT_MUL1 => 1,
                ReelExecuter.ETYPE.ADD_MONEY => 2,
                ReelExecuter.ETYPE.RANDOM => 0,
                _ => original,
            };
        }
        return original;
    }
}