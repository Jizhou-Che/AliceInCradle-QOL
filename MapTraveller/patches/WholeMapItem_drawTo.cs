using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;
using nel.mgm.fis;
using Better;
using m2d;
using XX;
using UnityEngine;

namespace MapTraveller.patches;

[HarmonyPatch(typeof(WholeMapItem), nameof(WholeMapItem.drawTo))]
public class WholeMapItem_drawTo
{
    private static readonly BDic<string, int> wmChipCount = [];
    public static readonly BDic<string, NelTreasureBoxDrawer.BOXTYPE> boxtype = [];

    static readonly AccessTools.FieldRef<WholeMapItem, List<WholeMapItem.WMItem>> AWmi = AccessTools.FieldRefAccess<WholeMapItem, List<WholeMapItem.WMItem>>("AWmi");
    static readonly AccessTools.FieldRef<WholeMapItem, List<Map2d>> Avisitted = AccessTools.FieldRefAccess<WholeMapItem, List<Map2d>>("Avisitted");
    static readonly AccessTools.FieldRef<NightController, BDic<string, NightController.SummonerData>> OSmnData = AccessTools.FieldRefAccess<NightController, BDic<string, NightController.SummonerData>>("OSmnData");

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(WholeMapItem), "AWmi")), new CodeMatch(OpCodes.Callvirt), new CodeMatch(OpCodes.Stloc_S));
        if (!codeMatcher.IsValid)
        {
            Plugin.Logger.LogError("IL matching failure in WholeMapItem_drawTo transpiler.");
            return codeMatcher.InstructionEnumeration();
        }
        codeMatcher.Advance(1);
        codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0), Transpilers.EmitDelegate(TravelMap));

        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(COOK), nameof(COOK.getSF), [typeof(string)])), new CodeMatch(OpCodes.Brfalse));
        if (!codeMatcher.IsValid)
        {
            Plugin.Logger.LogError("IL matching failure in WholeMapItem_drawTo transpiler.");
            return codeMatcher.InstructionEnumeration();
        }
        codeMatcher.Advance(1);
        codeMatcher.Set(OpCodes.Pop, null);
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(WholeMapItem.WMTransferPoint.WMRectItem), "no_exit_wm")), new CodeMatch(OpCodes.Brtrue));
        if (!codeMatcher.IsValid)
        {
            Plugin.Logger.LogError("IL matching failure in WholeMapItem_drawTo transpiler.");
            return codeMatcher.InstructionEnumeration();
        }
        codeMatcher.Advance(1);
        codeMatcher.Set(OpCodes.Pop, null);

        return codeMatcher.InstructionEnumeration();
    }

    static void TravelMap(WholeMapItem instance)
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            BDic<string, WAManager.WARecord> ORec = AccessTools.StaticFieldRefAccess<BDic<string, WAManager.WARecord>>(typeof(WAManager), "ORec");
            if (!wmChipCount.ContainsKey(instance.text_key))
            {
                wmChipCount.Add(instance.text_key, 0);
            }
            foreach (KeyValuePair<string, WAManager.WARecord> keyValuePair in ORec)
            {
                keyValuePair.Value.Touch("_whole_" + keyValuePair.Key, true, false, false);
            }

            List<M2LabelPoint> lpSummonList = [];
            List<M2LabelPoint> lpPuzzTreasureList = [];
            List<M2LabelPoint> lpItemSupplierList = [];
            List<M2LabelPoint> lpFishPondList = [];
            List<M2Puts> cpBenchList = [];
            List<M2Puts> cpCoffeemakerList = [];
            List<M2Puts> cpPuppetList = [];
            List<M2Puts> cpTildeList = [];
            for (int i = 0; i < AWmi(instance).Count; i++)
            {
                WholeMapItem.WMItem wmitem = AWmi(instance)[i];
                wmitem.visitted = true;
                Map2d srcMap = wmitem.SrcMap;
                if (!Avisitted(instance).Contains(srcMap))
                {
                    Avisitted(instance).Add(srcMap);
                }

                bool openedMap = false;
                if (!srcMap.loaded)
                {
                    srcMap.open(instance.M2D.GobBase, MAPMODE.CLOSED, null);
                    instance.M2D.initMapMaterialASync(srcMap, 0, false);
                    openedMap = true;
                }

                srcMap.getLabelPointAll(V => V is M2LpSummon, lpSummonList);
                srcMap.getLabelPointAll(V => V is M2LpPuzzTreasure, lpPuzzTreasureList);
                srcMap.getLabelPointAll(V => V is M2LpItemSupplier && !new META(V.comment).GetB("no_reel", false), lpItemSupplierList);
                srcMap.getLabelPointAll(V => V is M2LpFishPond, lpFishPondList);
                foreach (M2MapLayer layer in srcMap.getLayerArray())
                {
                    layer.getAllMetaPutsTo(cpBenchList, V => V is NelChipBench);
                    layer.getAllMetaPutsTo(cpCoffeemakerList, V => V is NelChipWanderNpcSpot && V is not NelChipWanderPuppet && V is not NelChipWanderTilde);
                    layer.getAllMetaPutsTo(cpPuppetList, V => V is NelChipWanderPuppet);
                    layer.getAllMetaPutsTo(cpTildeList, V => V is NelChipWanderTilde);
                }

                if (openedMap)
                {
                    srcMap.close(true, true);
                }
            }

            int iconCount = lpSummonList.Count + lpPuzzTreasureList.Count + lpItemSupplierList.Count + lpFishPondList.Count + cpBenchList.Count + cpCoffeemakerList.Count + cpPuppetList.Count + cpTildeList.Count;
            if (wmChipCount[instance.text_key] == 0 || iconCount < wmChipCount[instance.text_key])
            {
                if (Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.Slash) || Input.GetKey(KeyCode.LeftBracket) || Input.GetKey(KeyCode.RightBracket) || Input.GetKey(KeyCode.Backslash) || Input.GetKey(KeyCode.Semicolon) || Input.GetKey(KeyCode.Quote))
                {
                    UILog.Instance.AddAlert("首次加载地图需要时间，请几秒后再尝试绘制图标。");
                }

                if (iconCount > wmChipCount[instance.text_key])
                {
                    wmChipCount[instance.text_key] = iconCount;
                }
            }
            else
            {
                wmChipCount[instance.text_key] = iconCount;

                if (Input.GetKey(KeyCode.Comma))
                {
                    foreach (M2LabelPoint lp in lpSummonList)
                    {
                        EnemySummoner enemySummoner = EnemySummoner.Get(EnemySummoner.Lp2smn(lp.key), false);
                        if (enemySummoner != null && !enemySummoner.do_not_open)
                        {
                            M2LpSummon m2LpSummon = (M2LpSummon)lp;
                            WMIconCreator wmiconCreator = new(lp, WMIcon.TYPE.ENEMY, m2LpSummon.cleared_sf_key);
                            if (!wmiconCreator.getIcon().noticed)
                            {
                                wmiconCreator.notice();
                            }
                            NightController nightCon = ((NelM2DBase)M2DBase.Instance).NightCon;
                            NightController.SummonerData summonerData = nightCon.GetLpInfo(m2LpSummon, false);
                            OSmnData(nightCon)[m2LpSummon.cleared_sf_key] = summonerData;
                            summonerData.defeat_count++;
                        }
                    }
                }

                if (Input.GetKey(KeyCode.Period))
                {
                    foreach (M2LabelPoint lp in lpPuzzTreasureList)
                    {
                        M2LpPuzzTreasure m2LpPuzzTreasure = (M2LpPuzzTreasure)lp;
                        WMIconCreator wmiconCreator = new(m2LpPuzzTreasure, m2LpPuzzTreasure.is_mimic ? WMIcon.TYPE.TREASURE_MIMIC : WMIcon.TYPE.TREASURE, m2LpPuzzTreasure.box_sf_key);
                        if (!wmiconCreator.getIcon().noticed)
                        {
                            wmiconCreator.notice();
                            if (!boxtype.ContainsKey(m2LpPuzzTreasure.box_sf_key))
                            {
                                m2LpPuzzTreasure.Meta = new META(m2LpPuzzTreasure.comment);
                                if (m2LpPuzzTreasure.Meta.GetB("auto", false))
                                {
                                    TreasureData.PrepareTreasureMeta(m2LpPuzzTreasure, m2LpPuzzTreasure.Meta);
                                }
                                string type = m2LpPuzzTreasure.Meta.GetS("type");
                                bool flush = m2LpPuzzTreasure.Meta.GetI("flush", 0, 0) != 0;
                                PrSkill prSkill = SkillManager.Get(m2LpPuzzTreasure.Meta.GetS("skill") ?? "");
                                NelItem nelItem = NelItem.GetById(m2LpPuzzTreasure.Meta.GetS("item"), false);
                                if (TX.noe(type) || type.ToUpper() == "DEACTIVATE")
                                {
                                    if (!TX.noe(m2LpPuzzTreasure.Meta.GetS("mgkind")))
                                    {
                                        type = m2LpPuzzTreasure.Meta.GetB("mgkind_t", false) ? "M2D_MAGIC_T" : "M2D_MAGIC";
                                    }
                                    else if (prSkill != null)
                                    {
                                        if ((prSkill.category & SkillManager.SKILL_CTG.SPECIAL) != 0)
                                        {
                                            type = "FORBIDDEN";
                                        }
                                        else if ((prSkill.category & SkillManager.SKILL_CTG.HPMP) == SkillManager.SKILL_CTG.HP)
                                        {
                                            type = "M2D_SKILL_HP";
                                        }
                                        else if ((prSkill.category & SkillManager.SKILL_CTG.HPMP) == SkillManager.SKILL_CTG.MP)
                                        {
                                            type = "M2D_SKILL_MP";
                                        }
                                        else if ((prSkill.category & SkillManager.SKILL_CTG.ONLY_ALICE) != 0)
                                        {
                                            type = "FORBIDDEN_SKILL";
                                        }
                                        else
                                        {
                                            type = "M2D_SKILL";
                                        }
                                    }
                                    else if (m2LpPuzzTreasure.Meta.GetI("money", 0, 0) > 0)
                                    {
                                        type = flush ? "M2D_MONEY_FLUSH" : "M2D_MONEY";
                                    }
                                    else if (nelItem != null && nelItem.is_enhancer)
                                    {
                                        type = "M2D_ENHANCER";
                                    }
                                    else
                                    {
                                        type = flush ? "M2D_FLUSH" : "M2D_NORMAL";
                                    }
                                }
                                NelTreasureBoxDrawer.BOXTYPE bt = NelTreasureBoxDrawer.BOXTYPE.M2D_NORMAL;
                                FEnum<NelTreasureBoxDrawer.BOXTYPE>.TryParse(type, out bt, true, false);
                                boxtype.Add(m2LpPuzzTreasure.box_sf_key, bt);
                            }
                        }
                    }
                }

                if (Input.GetKey(KeyCode.Slash))
                {
                    foreach (M2Puts bench in cpBenchList)
                    {
                        ((NelChipBench)bench).fineIcon();
                    }
                }

                if (Input.GetKey(KeyCode.LeftBracket))
                {
                    foreach (M2Puts coffeemaker in cpCoffeemakerList)
                    {
                        new WMIconCreator((M2Chip)coffeemaker, WMIcon.TYPE.OTHER, "Coffeemaker|" + coffeemaker.Mp.key + "|" + coffeemaker.unique_key).notice();
                    }
                }

                if (Input.GetKey(KeyCode.RightBracket))
                {
                    foreach (M2Puts puppet in cpPuppetList)
                    {
                        new WMIconCreator((M2Chip)puppet, WMIcon.TYPE.OTHER, "Puppet|" + puppet.Mp.key + "|" + puppet.unique_key).notice();
                    }
                }

                if (Input.GetKey(KeyCode.Backslash))
                {
                    foreach (M2Puts tilde in cpTildeList)
                    {
                        new WMIconCreator((M2Chip)tilde, WMIcon.TYPE.OTHER, "Tilde|" + tilde.Mp.key + "|" + tilde.unique_key).notice();
                    }
                }

                if (Input.GetKey(KeyCode.Semicolon))
                {
                    foreach (M2LabelPoint lp in lpItemSupplierList)
                    {
                        new WMIconCreator(lp, WMIcon.TYPE.OTHER, "ItemSupplier|" + lp.Mp.key + "|" + lp.unique_key).notice();
                    }
                }

                if (Input.GetKey(KeyCode.Quote))
                {
                    foreach (M2LabelPoint lp in lpFishPondList)
                    {
                        new WMIconCreator(lp, WMIcon.TYPE.OTHER, "FishPond|" + lp.Mp.key + "|" + lp.unique_key).notice();
                    }
                }
            }
        }
    }
}