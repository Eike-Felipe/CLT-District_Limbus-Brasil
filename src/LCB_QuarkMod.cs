using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Il2CppSystem.Runtime.Remoting.Messaging;
using StorySystem;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LimbusCompanyPortuguêsBrasileiro
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class LCB_QuarkMod : BasePlugin
    {
        public static ConfigFile Quark_Settings;
        public static string ModPath;
        public static string GamePath;
        public const string GUID = "com.Quark.LimbusCompanyBrazilian";
        public const string NAME = "LimbusLocalizeBrasileiro";
        public const string VERSION = "0.1.0";
        public const string VERSION_STATE = "-PRE-ALPHA";
        public const string AUTHOR = "Base: Bright\nModified by: Knightey, abcdcode, Disaer";
        public const string QuarkLink = "";
        public static Action<string, Action> LogFatalError { get; set; }
        public static Action<string> LogInfo { get; set; }
        public static Action<string> LogError { get; set; }
        public static Action<string> LogWarning { get; set; }
        public static void OpenQuarkURL() => Application.OpenURL(QuarkLink);
        public static void OpenGamePath() => Application.OpenURL(GamePath);
        public override void Load()
        {
            Quark_Settings = Config;
            LogInfo = (string log) => { Log.LogInfo(log); Debug.Log(log); };
            LogError = (string log) => { Log.LogError(log); Debug.LogError(log); };
            LogWarning = (string log) => { Log.LogWarning(log); Debug.LogWarning(log); };
            LogFatalError = (string log, Action action) => { Manager.FatalErrorlog += log + "\n"; LogError(log); Manager.FatalErrorAction = action; Manager.CheckModActions(); };
            GamePath = new DirectoryInfo(Application.dataPath).Parent.FullName;
            var matchingFiles = Directory.EnumerateFiles(GamePath + "\\BepInEx\\plugins", "LimbusCompanyBus_pt-BR_BIE.dll", SearchOption.AllDirectories);
            foreach (var filePath in matchingFiles)
            {
                ModPath = Path.GetDirectoryName(filePath);
            }
            UpdateChecker.StartAutoUpdate();
            try
            {
                HarmonyLib.Harmony harmony = new(NAME);
                if (Brazilian_Settings.IsUseBrazilian.Value)
                {
                    Manager.InitLocalizes(new DirectoryInfo(ModPath + "/Localize/BR"));
                    harmony.PatchAll(typeof(Brazilian_Fonts));
                    harmony.PatchAll(typeof(ReadmeManager));
                    harmony.PatchAll(typeof(LoadingManager));
                    harmony.PatchAll(typeof(TemporaryTextures));
                    harmony.PatchAll(typeof(SpriteUI));
                    harmony.PatchAll(typeof(TextUI));
                    harmony.PatchAll(typeof(StoryUI));
                    //harmony.PatchAll(typeof(CreditsUI)); // WILL BEADDED LATER
                    //harmony.PatchAll(typeof(EventUI)); //WILL BEADDED LATER
                    harmony.PatchAll(typeof(SeasonUI));
                }
                harmony.PatchAll(typeof(Manager));
                harmony.PatchAll(typeof(Brazilian_Settings));
                if (!Brazilian_Fonts.AddBrazilianFont(ModPath + "/tmplatinfonts"))
                    LogFatalError("You have forgotten to install Font Update Mod. Please, reread README on Github.", OpenQuarkURL);
                LogInfo(AUTHOR);
                LogInfo("-------------------------\n");
                LogInfo("Startup" + DateTime.Now);
                //LogInfo("EventEnd" + new DateTime(2024, 9, 12, 2, 59, 0).ToLocalTime());
            }
            catch (Exception e)
            {
                LogFatalError("Mod has met an unknown fatal error! Contact us through urls in Github with the log, please.", () => { CopyLog(); OpenGamePath(); OpenQuarkURL(); });
                LogError(e.ToString());
            }
        }
        public static void CopyLog()
        {
            File.Copy(GamePath + "/BepInEx/LogOutput.log", GamePath + "/Latest.log", true);
            File.Copy(Application.consoleLogPath, GamePath + "/Player.log", true);
        }
    }
}