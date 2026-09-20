using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;
using XX;

namespace MapTraveller.patches;

[HarmonyPatch(typeof(WMIcon), nameof(WMIcon.drawTo))]
public class WMIcon_drawTo
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldarg_1), new CodeMatch(OpCodes.Ldarg_3), new CodeMatch(OpCodes.Ldarg_S), new CodeMatch(_ => true), new CodeMatch(_ => true), new CodeMatch(_ => true), new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Call), new CodeMatch(OpCodes.Brtrue), new CodeMatch(OpCodes.Ldstr, "wmap_treasure"), new CodeMatch(OpCodes.Br), new CodeMatch(OpCodes.Ldstr, "wmap_treasure_cleared"));
        if (!codeMatcher.IsValid)
        {
            Plugin.Logger.LogError("IL matching failure in WMIcon_drawTo transpiler.");
            return codeMatcher.InstructionEnumeration();
        }
        var label = codeMatcher.InstructionAt(22).operand;
        codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Br, label));
        return codeMatcher.InstructionEnumeration();
    }

    static void Postfix(WMIcon __instance, MeshDrawer Md, float lx, float ty, float alpha)
    {
        if (__instance.type == WMIcon.TYPE.TREASURE)
        {
            Md.Col = C32.WMulA(alpha);
            if (WholeMapItem_drawTo.boxtype.ContainsKey(__instance.sf_key))
            {
                Md.Col = WholeMapItem_drawTo.boxtype[__instance.sf_key] switch
                {
                    NelTreasureBoxDrawer.BOXTYPE.M2D_SKILL_HP => C32.MulA(4293170102U, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_SKILL_MP => C32.MulA(4284916479U, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_SKILL => C32.MulA(4283299675U, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_ENHANCER => C32.MulA(4288192204U, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_MONEY or NelTreasureBoxDrawer.BOXTYPE.M2D_MONEY_FLUSH => C32.MulA(4293918581U, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.FORBIDDEN or NelTreasureBoxDrawer.BOXTYPE.FORBIDDEN_SKILL => C32.MulA(4282992969U, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_MAGIC or NelTreasureBoxDrawer.BOXTYPE.M2D_MAGIC_T => C32.WMulA(0.5f),
                    _ => C32.WMulA(alpha),
                };
            }
            Md.RotaPF(lx, ty, 1f, 1f, 0f, MTRX.getPF(__instance.cleared ? "wmap_treasure_cleared" : "wmap_treasure"), false, false, false, uint.MaxValue, false, 0, false);
        }

        if (__instance.type == WMIcon.TYPE.OTHER)
        {
            switch (__instance.sf_key.Split('|')[0])
            {
                case "Coffeemaker":
                    Md.Col = C32.MulA(4278190080U, alpha);
                    Md.RotaPF(lx, ty, 0.6f, 0.6f, 0f, MTRX.getPF("itemrow_category.53"), false, false, false, uint.MaxValue, false, 0, false);
                    break;
                case "Puppet":
                    Md.Col = C32.WMulA(alpha);
                    Md.RotaPF(lx, ty, 0.7f, 0.7f, 0f, MTRX.getPF("IconGolem"), false, false, false, uint.MaxValue, false, 0, false);
                    break;
                case "Tilde":
                    Md.Col = C32.WMulA(alpha);
                    Md.RotaPF(lx, ty, 0.9f, 0.9f, 0f, MTRX.getPF("IconTilde"), false, false, false, uint.MaxValue, false, 0, false);
                    break;
                case "ItemSupplier":
                    Md.Col = C32.MulA(4278190080U, alpha);
                    Md.RotaPF(lx, ty, 1f, 1f, 0f, MTRX.getPF("itemrow_category.0"), false, false, false, uint.MaxValue, false, 0, false);
                    break;
                case "FishPond":
                    Md.Col = C32.MulA(4278190080U, alpha);
                    Md.RotaPF(lx, ty, 0.7f, 0.7f, 0f, MTRX.getPF("itemrow_category.79"), false, false, false, uint.MaxValue, false, 0, false);
                    break;
            }
        }
    }
}