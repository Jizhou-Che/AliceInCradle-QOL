using System.Collections.Generic;
using System.Reflection;
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
            Plugin.Logger.LogError($"IL matching failure in {MethodBase.GetCurrentMethod().DeclaringType.Name} transpiler.");
            return codeMatcher.InstructionEnumeration();
        }
        var label = codeMatcher.InstructionAt(22).operand;
        codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Br, label));
        return codeMatcher.InstructionEnumeration();
    }

    static void Postfix(WMIcon __instance, MeshDrawer Md, float lx, float ty, float alpha)
    {
        if (__instance.type == WMIcon.TYPE.TREASURE && __instance.sf_key != null)
        {
            Md.Col = C32.WMulA(alpha);
            if (WholeMapItem_drawTo.boxtype.ContainsKey(__instance.sf_key))
            {
                Md.Col = WholeMapItem_drawTo.boxtype[__instance.sf_key] switch
                {
                    NelTreasureBoxDrawer.BOXTYPE.M2D_SKILL_HP => C32.MulA(0xFFE493B6, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_SKILL_MP => C32.MulA(0xFF66A2FF, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_SKILL => C32.MulA(0xFF4DF75B, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_ENHANCER => C32.MulA(0xFF989ECC, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_MONEY or NelTreasureBoxDrawer.BOXTYPE.M2D_MONEY_FLUSH => C32.MulA(0xFFEFFF75, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.FORBIDDEN or NelTreasureBoxDrawer.BOXTYPE.FORBIDDEN_SKILL => C32.MulA(0xFF494949, alpha),
                    NelTreasureBoxDrawer.BOXTYPE.M2D_MAGIC or NelTreasureBoxDrawer.BOXTYPE.M2D_MAGIC_T => C32.WMulA(0.5f),
                    _ => C32.WMulA(alpha),
                };
            }
            Md.RotaPF(lx, ty, 1f, 1f, 0f, MTRX.getPF(__instance.cleared ? "wmap_treasure_cleared" : "wmap_treasure"), false, false, false, uint.MaxValue, false, 0, false);
        }

        if (__instance.type == WMIcon.TYPE.OTHER && __instance.sppos_key != null)
        {
            switch (__instance.sppos_key.Split('|')[0])
            {
                case "Coffeemaker":
                    Md.Col = C32.MulA(0xFF000000, alpha);
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
                    Md.Col = C32.MulA(0xCC000000, alpha);
                    Md.RotaPF(lx, ty, 1f, 1f, 0f, MTRX.getPF("itemrow_category.0"), false, false, false, uint.MaxValue, false, 0, false);
                    break;
                case "FishPond":
                    Md.Col = C32.MulA(0xFF000000, alpha);
                    Md.RotaPF(lx, ty, 0.7f, 0.7f, 0f, MTRX.getPF("itemrow_category.79"), false, false, false, uint.MaxValue, false, 0, false);
                    break;
            }
        }
    }
}
