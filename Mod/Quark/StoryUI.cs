using HarmonyLib;
using StorySystem.InterEffect;
using StorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MainUI;
using UtilityUI;
using BattleUI;
using UI.Utility;
using Il2CppSystem.Text.RegularExpressions;

namespace LimbusCompanyPortuguêsBrasileiro
{
    public static class StoryUI
    {
        #region Story
        [HarmonyPatch(typeof(StoryManager), nameof(StoryManager.Init))]
        [HarmonyPostfix]
        private static void StoryManager_SetData(StoryManager __instance)
        {
            __instance._dialogCon.tmp_name.lineSpacing = -25;
        }
        #endregion

        #region Diary
        [HarmonyPatch(typeof(BookPageContent), nameof(BookPageContent.GetShowTextSequence))]
        [HarmonyPrefix]
        private static void Diary_Book(BookPageContent __instance)
        {
            if (__instance.tmp_content != null)
            {
                __instance.tmp_content.m_fontAsset = Brazilian_Fonts.tmplatinfonts[1];
                __instance.tmp_content.m_sharedMaterial = Brazilian_Fonts.tmplatinfonts[1].material;
                __instance.tmp_content.SetAllDirty();
                __instance.tmp_content.fontSize = 46f;
                __instance.tmp_content.lineSpacing = -30f;
            }
            if (__instance.tmp_title != null)
            {
                __instance.tmp_title.m_fontAsset = Brazilian_Fonts.tmplatinfonts[1];
                __instance.tmp_title.m_sharedMaterial = Brazilian_Fonts.tmplatinfonts[1].material;
                __instance.tmp_title.SetAllDirty();
                __instance.tmp_title.fontSize = 54f;
                __instance.tmp_title.lineSpacing = -25f;
                __instance.tmp_title.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(__instance.tmp_title.GetComponentInChildren<RectTransform>().anchoredPosition.x, -85f);
            }
            if (__instance.tmp_titleReference != null)
            {
                __instance.tmp_titleReference.m_fontAsset = Brazilian_Fonts.tmplatinfonts[1];
                __instance.tmp_titleReference.m_sharedMaterial = Brazilian_Fonts.tmplatinfonts[1].material;
                __instance.tmp_titleReference.SetAllDirty();
                __instance.tmp_titleReference.fontSize = 54f;
                __instance.tmp_titleReference.lineSpacing = -25f;
                __instance.tmp_titleReference.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(__instance.tmp_titleReference.GetComponentInChildren<RectTransform>().anchoredPosition.x, -85f);
            }
        }
        #endregion

        #region Clear All Cathy Fake Screen
        [HarmonyPatch(typeof(StoryInterEffect_Type1), nameof(StoryInterEffect_Type1.Initialize))]
        [HarmonyPostfix]
        private static void StoryInterEffect_Type1_Init(StoryInterEffect_Type1 __instance)
        {
            //FAKE_TITLE
            Transform title = __instance._title.transform;
            Transform motto = title.Find("[Canvas]/[Image]RedLine/[Image]Phrase");
            Transform logo = title.Find("[Image]Logo");
            SpriteUI.Motto_Changer(null, logo, motto);
            Image donttouch = title.Find("[Canvas]/[Image]TouchToStart").GetComponentInChildren<Image>();
            donttouch.m_OverrideSprite = ReadmeManager.GetReadmeStorySprites("Don't_Start");
            Transform goldenbough = title.Find("[Canvas]/[Text]GoldenBoughSynchronized");
            Transform goldenbough_glow = title.Find("[Canvas]/[Text]GoldenBoughSynchronized/[Text]Glow");
            List<TextMeshProUGUI> goldens_text = new List<TextMeshProUGUI> { goldenbough.GetComponentInChildren<TextMeshProUGUI>(), goldenbough_glow.GetComponentInChildren<TextMeshProUGUI>() };
            foreach (TextMeshProUGUI t in goldens_text)
            {
                t.text = "RAMO DOURADO SINCRONIZADO";
            }
            goldenbough_glow.GetComponentInChildren<TextMeshProUGUI>(true).alpha = 0.25f;
            //FAKE_LOADING
            Transform loading = __instance._loading.transform;
            TextMeshProUGUI now_l = loading.Find("[Rect]LoadingUI/Text_NowLoading").transform.GetComponentInChildren<TextMeshProUGUI>();
            now_l.text = "CARREGANDO...";
            TextMeshProUGUI clearing = loading.Find("[Rect]LoadingUI/[Text]ProgressCategory").transform.GetComponentInChildren<TextMeshProUGUI>();
            clearing.text = "LIMPANDO TODO O CACHY";
            TextMeshProUGUI clearing_glow = loading.Find("[Rect]LoadingUI/[Text]ProgressCategory/[Text]ProgressCategoryGlow").transform.GetComponentInChildren<TextMeshProUGUI>();
            clearing_glow.text = "LIMPANDO TODO O CACHY";
        }
        #endregion

        #region Heath's Cathy Dialogue Censorship
        [HarmonyPatch(typeof(Util), nameof(Util.GetDlgAfterClearingAllCathy))]
        [HarmonyPrefix]
        private static bool GetDlgAfterClearingAllCathy(string dlgId, string originString, ref string __result)
        {
            if (Brazilian_Settings.IsUseBrazilian.Value)
            {
                __result = originString;
                UserDataManager instance = Singleton<UserDataManager>.Instance;
                if (instance == null || instance._unlockCodeData == null || !instance._unlockCodeData.CheckUnlockStatus(106))
                    return false;
                if ("battle_defeat_10707_1".Equals(dlgId))
                    __result = __result.Replace("Cathy", "■■■■■");
                else if ("battle_dead_10704_1".Equals(dlgId))
                    __result = __result.Replace("Catherine", "■■■■■■■■■");
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(StoryPlayData), nameof(StoryPlayData.GetDialogAfterClearingAllCathy))]
        [HarmonyPrefix]
        private static bool GetDialogAfterClearingAllCathy(Scenario curStory, Dialog dialog, ref string __result)
        {
            if (Brazilian_Settings.IsUseBrazilian.Value)
            {
                __result = dialog.Content;
                UserDataManager instance = Singleton<UserDataManager>.Instance;
                if ("P10704".Equals(curStory.ID) && instance != null && instance._unlockCodeData != null && instance._unlockCodeData.CheckUnlockStatus(106) && dialog.Id == 3)
                {
                    __result = __result.Replace("Cathy", "■■■■■");
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Dante Ability
        [HarmonyPatch(typeof(DanteAbilityUIController), nameof(DanteAbilityUIController.UpdatePopup))]
        [HarmonyPostfix]
        private static void DanteAbilityUI_TitleChanger(DanteAbilityUIController __instance)
        {
            __instance._titleText.characterSpacing = 2;
        }

        [HarmonyPatch(typeof(DanteAbilityUIController), nameof(DanteAbilityUIController.SetActivePopup))]
        [HarmonyPostfix]
        private static void DanteAbilityUIController_Sefiroth(DanteAbilityUIController __instance)
        {
            foreach (var slot in __instance._showAbilitySlotList)
            {
                var caution = slot._graphicList[4].GetComponentInChildren<TextMeshProUGUI>();
                caution.text = "CUIDADO";
            }
        }
        [HarmonyPatch(typeof(DanteAbilitySlot), nameof(DanteAbilitySlot.SetDescActive))]
        [HarmonyPostfix]
        private static void DanteAbilityUIController_Description(DanteAbilitySlot __instance)
        {
            if (__instance._nameText.enabled)
            {
                __instance._rawDescText.color = __instance._nameText.color;
            }
        }
        [HarmonyPatch(typeof(EnemyHudToggle), nameof(EnemyHudToggle.SetCurrentState))]
        [HarmonyPostfix]
        private static void DanteAbility_KillCount_Enemy(EnemyHudToggle __instance)
        {
            __instance._sinButton.tmp_text.text = "<size=70%><nobr>AFINIDADES</nobr> DE PECADOR</size>";
            __instance._enemyPassiveButton.tmp_text.text = "<size=70%><nobr>PASSIVAS</nobr> DE INIMIGOS</size>";
            __instance._enemyPassiveButton.tmp_text.lineSpacing = 10;
        }
        #endregion
    }
}
