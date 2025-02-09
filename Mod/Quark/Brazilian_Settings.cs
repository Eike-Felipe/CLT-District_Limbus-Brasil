using HarmonyLib;
using LocalSave;
using MainUI;
using BepInEx.Configuration;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LimbusCompanyPortuguêsBrasileiro
{
    public static class Brazilian_Settings
    {
        public static ConfigEntry<bool> IsUseBrazilian = LCB_QuarkMod.Quark_Settings.Bind("Quark Settings", "IsUseBrazilian", true, "true por padrão, false é opcional");
        static bool _isusebrazilian;
        static Toggle Brazil_Settings;
        [HarmonyPatch(typeof(SettingsPanelGame), nameof(SettingsPanelGame.InitLanguage))]
        [HarmonyPrefix]
        private static bool InitLanguage(SettingsPanelGame __instance, LocalGameOptionData option)
        {
            if (!Brazil_Settings)
            {
                Toggle original = __instance._languageToggles[0];
                Transform parent = original.transform.parent;
                var _languageToggle = UnityEngine.Object.Instantiate(original, parent);
                var rutmp = _languageToggle.GetComponentInChildren<TextMeshProUGUI>(true);
                rutmp.font = Brazilian_Fonts.GetBrazilianFonts(4);
                rutmp.fontMaterial = Brazilian_Fonts.GetBrazilianFonts(4).material;
                rutmp.text = "<size=60%><cspace=-4px>Português Brasileiro</cspace></size>";
                Brazil_Settings = _languageToggle;
                parent.localPosition = new Vector3(parent.localPosition.x - 306f, parent.localPosition.y, parent.localPosition.z);
                while (__instance._languageToggles.Count > 3)
                    __instance._languageToggles.RemoveAt(__instance._languageToggles.Count - 1);
                __instance._languageToggles.Add(_languageToggle);
            }
            foreach (Toggle tg in __instance._languageToggles)
            {
                tg.onValueChanged.RemoveAllListeners();
                Action<bool> onValueChanged = (bool isOn) =>
                {
                    if (!isOn)
                        return;
                    __instance.OnClickLanguageToggleEx(__instance._languageToggles.IndexOf(tg));
                };
                tg.onValueChanged.AddListener(onValueChanged);
                tg.SetIsOnWithoutNotify(false);
            }
            LOCALIZE_LANGUAGE language = option.GetLanguage();
            if (_isusebrazilian = IsUseBrazilian.Value)
                Brazil_Settings.SetIsOnWithoutNotify(true);
            else if (language == LOCALIZE_LANGUAGE.KR)
                __instance._languageToggles[0].SetIsOnWithoutNotify(true);
            else if (language == LOCALIZE_LANGUAGE.EN)
                __instance._languageToggles[1].SetIsOnWithoutNotify(true);
            else if (language == LOCALIZE_LANGUAGE.JP)
                __instance._languageToggles[2].SetIsOnWithoutNotify(true);
            __instance._lang = language;
            return false;
        }
        [HarmonyPatch(typeof(SettingsPanelGame), nameof(SettingsPanelGame.ApplySetting))]
        [HarmonyPostfix]
        private static void ApplySetting() => IsUseBrazilian.Value = _isusebrazilian;
        private static void OnClickLanguageToggleEx(this SettingsPanelGame __instance, int tgIdx)
        {
            if (tgIdx == 3)
            {
                _isusebrazilian = true;
                return;
            }
            _isusebrazilian = false;
            if (tgIdx == 0)
                __instance._lang = LOCALIZE_LANGUAGE.KR;
            else if (tgIdx == 1)
                __instance._lang = LOCALIZE_LANGUAGE.EN;
            else if (tgIdx == 2)
                __instance._lang = LOCALIZE_LANGUAGE.JP;
        }
        [HarmonyPatch(typeof(DateUtil), nameof(DateUtil.TimeZoneOffset), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool TimeZoneOffset(ref int __result)
        {
            if (IsUseBrazilian.Value)
            {
                __result = -3;
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(DateUtil), nameof(DateUtil.TimeZoneString), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool TimeZoneString(ref string __result)
        {
            if (IsUseBrazilian.Value)
            {
                __result = "BRT";
                return false;
            }
            return true;
        }
    }
}
