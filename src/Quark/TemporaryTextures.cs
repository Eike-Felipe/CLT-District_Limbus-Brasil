using BattleUI;
using BattleUI.Information;
using HarmonyLib;
using MainUI;
using MainUI.VendingMachine;
using TMPro;
using UnityEngine;
using MainUI.Gacha;
using BattleUI.Typo;
using BattleUI.BattleUnit;
using System.Collections.Generic;
using UtilityUI;
using MainUI.BattleResult;
using Il2CppSystem.Security.Cryptography;

namespace LimbusCompanyPortuguêsBrasileiro
{
   public static class TemporaryTextures
    {
        #region Dialogues
        [HarmonyPatch(typeof(BattleResultUIPanel), nameof(BattleResultUIPanel.SetStatusUI))]
        [HarmonyPostfix]
        private static void BattleResult_Init(BattleResultUIPanel __instance)
        {
            __instance.tmp_result.lineSpacing = -30;
            __instance.tmp_result.characterSpacing = 3;
        }
        #endregion

        #region Battle
        [HarmonyPatch(typeof(BattleTotalDamageTypoSlot), nameof(BattleTotalDamageTypoSlot.Open))]
        [HarmonyPostfix]
        private static void BattleTotalDamageTypoSlot_Init(BattleTotalDamageTypoSlot __instance)
        {
            GameObject ego_t = GameObject.Find("[Script]BattleUIRoot/[Script]BattleSkillViewUIController/[Canvas]Canvas/[Script]SkillViewCanvas/[Rect]NameBox_EGO/[Image]EgoNameBg/[Rect]TotalDamageSlot/[Tmpro]Total");
            List<TextMeshProUGUI> vsego = new List<TextMeshProUGUI> { __instance.tmp_total, ego_t.GetComponentInChildren<TextMeshProUGUI>() };
            foreach (var v in vsego)
            {
                v.text = "<size=40>ВСЕГО</size>"; // NEEDS TRANSLATION, TOTAL
            }
        }
        [HarmonyPatch(typeof(TargetDetailSkillInfoController), nameof(TargetDetailSkillInfoController.SetUISetting))]
        [HarmonyPostfix]
        private static void Targeting(TargetDetailSkillInfoController __instance)
        {
            if (__instance._winRateTypo != null)
                __instance._winRateTypo._textMeshPro.lineSpacing = -30;
        }
        [HarmonyPatch(typeof(TargettingSkillInfo_Base), nameof(TargettingSkillInfo_Base.Init))]
        [HarmonyPrefix]
        private static void Targeting_SkillName(TargettingSkillInfo_Base __instance)
        {
            if (__instance != null)
            {
                __instance._tmp_skillName.GetComponentInChildren<TextMeshProLanguageSetter>(true).enabled = false;
                __instance._tmp_skillName.lineSpacing = -20;
            }
        }
        #endregion
    }
}
