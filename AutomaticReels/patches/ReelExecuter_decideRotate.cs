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
        codeMatcher.Advance(2);
        codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0), Transpilers.EmitDelegate(PushContentIdDec));
        return codeMatcher.InstructionEnumeration();
    }

    static int PushContentIdDec(int original, ReelExecuter instance)
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            switch (instance.getEType())
            {
                case ReelExecuter.ETYPE.GRADE1:
                    return 3;
                case ReelExecuter.ETYPE.GRADE2:
                    return 2;
                case ReelExecuter.ETYPE.GRADE3:
                    return 0;
                case ReelExecuter.ETYPE.COUNT_ADD1:
                    return 1;
                case ReelExecuter.ETYPE.COUNT_ADD2:
                    return 10;
                case ReelExecuter.ETYPE.COUNT_ADD3:
                    return 1;
                case ReelExecuter.ETYPE.COUNT_MUL1:
                    return 1;
                case ReelExecuter.ETYPE.ADD_MONEY:
                    return 2;
                case ReelExecuter.ETYPE.RANDOM:
                    return 0;
                default:
                    return original;
            }
        }

        return original;
    }
}