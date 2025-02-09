using HarmonyLib;
using MainUI;
using MainUI.Gacha;
using TMPro;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ILObject = Il2CppSystem.Object;
using UObject = UnityEngine.Object;

namespace LimbusCompanyPortuguêsBrasileiro
{
    public class Manager : MonoBehaviour
    {
        static Manager()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Manager>();
            GameObject obj = new(nameof(Manager));
            DontDestroyOnLoad(obj);
            obj.hideFlags |= HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<Manager>();
        }
        public static Manager Instance;
        public Manager(IntPtr ptr) : base(ptr) { }
        void OnApplicationQuit() => LCB_QuarkMod.CopyLog();
        public static void OpenGlobalPopup(string description, string title = null, string close = "Fechar", string confirm = "OK", Action confirmEvent = null, Action closeEvent = null)
        {
            if (!GlobalGameManager.Instance) { return; }
            TextOkUIPopup globalPopupUI = GlobalGameManager.Instance.globalPopupUI;
            TMP_FontAsset fontAsset = Brazilian_Fonts.GetBrazilianFonts(3);
            if (fontAsset)
            {
                TextMeshProUGUI btn_canceltmp = globalPopupUI.btn_cancel.GetComponentInChildren<TextMeshProUGUI>(true);
                btn_canceltmp.font = fontAsset;
                btn_canceltmp.fontMaterial = fontAsset.material;
                UITextDataLoader btn_canceltl = globalPopupUI.btn_cancel.GetComponentInChildren<UITextDataLoader>(true);
                btn_canceltl.enabled = false;
                btn_canceltmp.text = close;
                TextMeshProUGUI btn_oktmp = globalPopupUI.btn_ok.GetComponentInChildren<TextMeshProUGUI>(true);
                btn_oktmp.font = fontAsset;
                btn_oktmp.fontMaterial = fontAsset.material;
                UITextDataLoader btn_oktl = globalPopupUI.btn_ok.GetComponentInChildren<UITextDataLoader>(true);
                btn_oktl.enabled = false;
                btn_oktmp.text = confirm;
                globalPopupUI.tmp_title.font = fontAsset;
                globalPopupUI.tmp_title.fontMaterial = fontAsset.material;
                void TextLoaderEnabled() { btn_canceltl.enabled = true; btn_oktl.enabled = true; }
                confirmEvent += TextLoaderEnabled;
                closeEvent += TextLoaderEnabled;
            }
            globalPopupUI._titleObject.SetActive(!string.IsNullOrEmpty(title));
            globalPopupUI.tmp_title.text = title;
            globalPopupUI.tmp_description.text = description;
            globalPopupUI._confirmEvent = confirmEvent;
            globalPopupUI._closeEvent = closeEvent;
            globalPopupUI.btn_cancel.gameObject.SetActive(!string.IsNullOrEmpty(close));
            globalPopupUI._gridLayoutGroup.cellSize = new Vector2(!string.IsNullOrEmpty(close) ? 500 : 700, 100f);
            globalPopupUI.Open();
        }
        public static void InitLocalizes(DirectoryInfo directory)
        {
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                var value = File.ReadAllText(fileInfo.FullName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                Localizes[fileNameWithoutExtension] = value;
            }
            foreach (DirectoryInfo directoryInfo in directory.GetDirectories())
            {
                InitLocalizes(directoryInfo);
            }

        }
        public static Dictionary<string, string> Localizes = new();
        public static Action FatalErrorAction;
        public static string FatalErrorlog;
        #region Запрет предупреждений
        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.Log), typeof(LogType), typeof(ILObject))]
        [HarmonyPrefix]
        private static bool Log(UnityEngine.Logger __instance, LogType logType, ILObject message)
        {
            if (logType != LogType.Warning) return true;
            var logString = UnityEngine.Logger.GetString(message);
            if (!logString.StartsWith("<color=#0099bc><b>DOTWEEN"))
                __instance.logHandler.LogFormat(logType, null, "{0}", logString); 
            return false;
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.Log), typeof(LogType), typeof(ILObject), typeof(UObject))]
        [HarmonyPrefix]
        private static bool Log(UnityEngine.Logger __instance, LogType logType, ILObject message, UObject context)
        {
            if (logType != LogType.Warning) return true;
            var logString = UnityEngine.Logger.GetString(message);
            if (!logString.StartsWith("Material"))
                __instance.logHandler.LogFormat(logType, context, "{0}", logString);
            return false;
        }
        #endregion
        #region Исправление некоторых ошибок
        [HarmonyPatch(typeof(GachaEffectEventSystem), nameof(GachaEffectEventSystem.LinkToCrackPosition))]
        [HarmonyPrefix]
        private static bool LinkToCrackPosition(GachaEffectEventSystem __instance, GachaCrackController[] crackList)
            => __instance._parent.EffectChainCamera;
        #endregion
        [HarmonyPatch(typeof(LoginSceneManager), nameof(LoginSceneManager.SetLoginInfo))]
        [HarmonyPostfix]
        public static void CheckModActions()
        {
            if (UpdateChecker.UpdateCall != null)
                OpenGlobalPopup("Есть обновление" /* UPDATE IS AVAILABLE*/ + UpdateChecker.Updatelog + "Há uma atualização disponível!\nFeche o jogo e atualize o mod." + UpdateChecker.Updatelog + "Extraia os arquivos do mod", "O mod foi atualizado!", null, "OK", () =>
                {
                    UpdateChecker.UpdateCall.Invoke();
                    UpdateChecker.UpdateCall = null;
                    UpdateChecker.Updatelog = string.Empty;
                });
            else if (FatalErrorAction != null)
                OpenGlobalPopup(FatalErrorlog, "Ocorreu um erro!", null, "Ir para o GitHub", () =>
                {
                    FatalErrorAction.Invoke();
                    FatalErrorAction = null;
                    FatalErrorlog = string.Empty;
                });
        }
    }
}