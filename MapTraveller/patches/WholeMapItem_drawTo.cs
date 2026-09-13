using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;
using m2d;
using Better;
using UnityEngine;
using XX;

namespace MapTraveller.patches;

[HarmonyPatch(typeof(WholeMapItem), nameof(WholeMapItem.drawTo))]
public class WholeMapItem_drawTo
{
    private static readonly BDic<string, bool> wmFirstLoad = [];
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
            if (!wmFirstLoad.ContainsKey(instance.text_key))
            {
                wmFirstLoad.Add(instance.text_key, false);
            }
            foreach (KeyValuePair<string, WAManager.WARecord> keyValuePair in ORec)
            {
                keyValuePair.Value.Touch("_whole_" + keyValuePair.Key, true, false, false);
            }

            int count = AWmi(instance).Count;
            for (int i = 0; i < count; i++)
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

                if (wmFirstLoad[instance.text_key])
                {
                    if (Input.GetKey(KeyCode.Comma))
                    {
                        List<M2LabelPoint> labelPointAll = srcMap.getLabelPointAll(V => V is M2LpSummon, null);
                        if (labelPointAll != null)
                        {
                            foreach (M2LabelPoint labelPoint in labelPointAll)
                            {
                                EnemySummoner enemySummoner = EnemySummoner.Get(EnemySummoner.Lp2smn(labelPoint.key), false);
                                if (enemySummoner != null && !enemySummoner.do_not_open)
                                {
                                    M2LpSummon m2LpSummon = (M2LpSummon)labelPoint;
                                    WMIconCreator wmiconCreator = new(labelPoint, WMIcon.TYPE.ENEMY, m2LpSummon.cleared_sf_key);
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
                    }

                    if (Input.GetKey(KeyCode.Period))
                    {
                        List<M2LabelPoint> labelPointAll = srcMap.getLabelPointAll(V => V is M2LpPuzzTreasure, null);
                        if (labelPointAll != null)
                        {
                            foreach (M2LabelPoint labelPoint in labelPointAll)
                            {
                                M2LpPuzzTreasure m2LpPuzzTreasure = (M2LpPuzzTreasure)labelPoint;
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
                                        string item = m2LpPuzzTreasure.Meta.GetS("item");
                                        int money_count = m2LpPuzzTreasure.Meta.GetI("money", 0, 0);
                                        bool flush = m2LpPuzzTreasure.Meta.GetI("flush", 0, 0) != 0;
                                        NelItem nelItem = NelItem.GetById(item, false);
                                        if (TX.noe(type) || type.ToUpper() == "DEACTIVATE")
                                        {
                                            if (!TX.noe(m2LpPuzzTreasure.Meta.GetS("mgkind")))
                                            {
                                                type = m2LpPuzzTreasure.Meta.GetB("mgkind_t", false) ? "M2D_MAGIC_T" : "M2D_MAGIC";
                                            }
                                            else
                                            {
                                                PrSkill prSkill = SkillManager.Get(m2LpPuzzTreasure.Meta.GetS("skill") ?? "");
                                                if (prSkill != null)
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
                                                else if (money_count > 0)
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
                                        }
                                        NelTreasureBoxDrawer.BOXTYPE bt = NelTreasureBoxDrawer.BOXTYPE.M2D_NORMAL;
                                        FEnum<NelTreasureBoxDrawer.BOXTYPE>.TryParse(type, out bt, true, false);
                                        boxtype.Add(m2LpPuzzTreasure.box_sf_key, bt);
                                    }
                                }
                            }
                        }
                    }

                    if (Input.GetKey(KeyCode.Slash))
                    {
                        foreach (M2MapLayer m2MapLayer in srcMap.getLayerArray())
                        {
                            List<M2Puts> benches = null;
                            benches = m2MapLayer.getAllMetaPutsTo(benches, V => V is NelChipBench);
                            if (benches != null)
                            {
                                foreach (M2Puts bench in benches)
                                {
                                    ((NelChipBench)bench).fineIcon();
                                }
                            }
                        }
                    }

                    if (Input.GetKey(KeyCode.LeftBracket))
                    {
                        foreach (M2MapLayer m2MapLayer in srcMap.getLayerArray())
                        {
                            List<M2Puts> coffeemakers = null;
                            coffeemakers = m2MapLayer.getAllMetaPutsTo(coffeemakers, V => V is NelChipWanderNpcSpot && V is not NelChipWanderPuppet && V is not NelChipWanderTilde);
                            if (coffeemakers != null)
                            {
                                foreach (M2Puts coffeemaker in coffeemakers)
                                {
                                    new WMIconCreator((M2Chip)coffeemaker, WMIcon.TYPE.OTHER, "coffeemaker").notice();
                                }
                            }
                        }
                    }

                    if (Input.GetKey(KeyCode.RightBracket))
                    {
                        foreach (M2MapLayer m2MapLayer in srcMap.getLayerArray())
                        {
                            List<M2Puts> puppets = null;
                            puppets = m2MapLayer.getAllMetaPutsTo(puppets, V => V is NelChipWanderPuppet);
                            if (puppets != null)
                            {
                                foreach (M2Puts puppet in puppets)
                                {
                                    new WMIconCreator((M2Chip)puppet, WMIcon.TYPE.OTHER, "puppet").notice();
                                }
                            }
                        }
                    }

                    if (Input.GetKey(KeyCode.Backslash))
                    {
                        foreach (M2MapLayer m2MapLayer in srcMap.getLayerArray())
                        {
                            List<M2Puts> tildes = null;
                            tildes = m2MapLayer.getAllMetaPutsTo(tildes, V => V is NelChipWanderTilde);
                            if (tildes != null)
                            {
                                foreach (M2Puts tilde in tildes)
                                {
                                    new WMIconCreator((M2Chip)tilde, WMIcon.TYPE.OTHER, "tilde").notice();
                                }
                            }
                        }
                    }
                }

                if (openedMap)
                {
                    srcMap.close(true, true);
                }
            }

            if (!wmFirstLoad[instance.text_key])
            {
                wmFirstLoad[instance.text_key] = true;
                if (Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.Slash) || Input.GetKey(KeyCode.LeftBracket) || Input.GetKey(KeyCode.RightBracket) || Input.GetKey(KeyCode.Backslash))
                {
                    UILog.Instance.AddAlert("首次加载地图需要时间，请几秒后再尝试生成图标。");
                }
            }
        }
    }
}