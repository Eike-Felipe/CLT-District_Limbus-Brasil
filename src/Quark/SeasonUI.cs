using HarmonyLib;
using MainUI;
using MainUI.VendingMachine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UI;
using TMPro;
using static UI.Utility.InfoModels;
using static UI.Utility.TMProStringMatcher;

namespace LimbusCompanyPortuguêsBrasileiro
{
    public static class SeasonUI
    {
        [HarmonyPatch(typeof(BattlePassUIPopup), nameof(BattlePassUIPopup.SetupBaseData))]
        [HarmonyPostfix]
        private static void SeasonPass_Init(BattlePassUIPopup __instance)
        {
            __instance.seasonPeriod.font = Brazilian_Fonts.tmplatinfonts[1];
            __instance.seasonPeriod.fontMaterial = Brazilian_Fonts.tmplatinfonts[1].material;
            __instance.seasonPeriod.text = "(BRT) 00:00 10.10.2024 ~";

            //FLAGS
            __instance.seasonPeriod.m_isRebuildingLayout = false;
            __instance.seasonPeriod.ignoreVisibility = true;
            __instance.seasonPeriod.isOverlay = false;
            __instance.seasonPeriod.m_ignoreCulling = true;
            __instance.seasonPeriod.isOverlay = false;
            __instance.seasonPeriod.m_isOverlay = false;
            __instance.seasonPeriod.m_isParsingText = true;
            __instance.seasonPeriod.m_RaycastTarget = false;
            __instance.seasonPeriod.raycastTarget = false;
        }
    }
}
