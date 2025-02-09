using SimpleJSON;
using BepInEx.Configuration;
using Il2CppSystem.Threading;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LimbusCompanyPortuguêsBrasileiro
{
    public static class UpdateChecker
    {
        public static ConfigEntry<bool> AutoUpdate = LCB_QuarkMod.Quark_Settings.Bind("Quark Settings", "AutoUpdate", false, "");
        public static void StartAutoUpdate()
        {
            if (AutoUpdate.Value)
            {
                LCB_QuarkMod.LogWarning("Check Mod Update");
                Action ModUpdate = CheckModUpdate;
                new Thread(ModUpdate).Start();
            }
        }
        static void CheckModUpdate()
        {
            UnityWebRequest www = UnityWebRequest.Get("");
            www.timeout = 4;
            www.SendWebRequest();
            while (!www.isDone)
                Thread.Sleep(100);
            if (www.result != UnityWebRequest.Result.Success)
                LCB_QuarkMod.LogWarning("Conexão com GitHub falhou!" + www.error);
            else
            {
                JSONArray releases = JSONNode.Parse(www.downloadHandler.text).AsArray;
                string latestReleaseTag = releases[0]["tag_name"].Value;
                string latest2ReleaseTag = releases.m_List.Count > 1 ? releases[1]["tag_name"].Value : string.Empty;
                if (Version.Parse(LCB_QuarkMod.VERSION) < Version.Parse(latestReleaseTag.Remove(0, 1)))
                {
                    string updatelog = (latest2ReleaseTag == "v" + LCB_QuarkMod.VERSION ? "LimbusCompanyBus_pt-BR_BIE.7z" : "LimbusCompanyBus_pt-BR_BIE.zip") + latestReleaseTag;
                    string download = "" + latestReleaseTag + "/" + updatelog;
                    var dirs = download.Split('/');
                    string filename = LCB_QuarkMod.GamePath + "/" + dirs[^1];
                    if (!File.Exists(filename))
                        DownloadFileAsync(download, filename).GetAwaiter().GetResult();
                    UpdateCall = UpdateDel;
                }
                LCB_QuarkMod.LogWarning("Check Brazil Font Asset Update");
                Action FontAssetUpdate = CheckBrazilFontAssetUpdate;
                new Thread(FontAssetUpdate).Start();
            }
        }
        static void CheckBrazilFontAssetUpdate()
        {
            UnityWebRequest www = UnityWebRequest.Get("");
            string FilePath = LCB_QuarkMod.ModPath + "/tmplatinfonts";
            var LastWriteTime = File.Exists(FilePath) ? int.Parse(TimeZoneInfo.ConvertTime(new FileInfo(FilePath).LastWriteTime, TimeZoneInfo.FindSystemTimeZoneById("Brasília Time")).ToString("ddMMyy")) : 0;
            www.SendWebRequest();
            while (!www.isDone)
                Thread.Sleep(100);
            var latest = JSONNode.Parse(www.downloadHandler.text).AsObject;
            int latestReleaseTag = int.Parse(latest["tag_name"].Value);
            if (LastWriteTime < latestReleaseTag)
            {
                string download = "";
                var dirs = download.Split('/');
                string filename = LCB_QuarkMod.GamePath + "/" + dirs[^1];
                if (!File.Exists(filename))
                    DownloadFileAsync(download, filename).GetAwaiter().GetResult();
                UpdateCall = UpdateDel;
            }
        }
        static void UpdateDel()
        {
            LCB_QuarkMod.OpenGamePath();
            Application.Quit();
        }
        static async Task DownloadFileAsync(string url, string filePath)
        {
            LCB_QuarkMod.LogWarning("Download " + url + " To " + filePath);
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);
            using HttpContent content = response.Content;
            using FileStream fileStream = new(filePath, FileMode.Create);
            await content.CopyToAsync(fileStream);
        }
        public static void CheckReadmeUpdate()
        {
            UnityWebRequest www = UnityWebRequest.Get("");
            www.timeout = 1;
            www.SendWebRequest();
            string FilePath = LCB_QuarkMod.ModPath + "/Localize/Readme/Readme.json";
            var LastWriteTime = new FileInfo(FilePath).LastWriteTime;
            while (!www.isDone)
            {
                Thread.Sleep(100);
            }
            if (www.result == UnityWebRequest.Result.Success && LastWriteTime < DateTime.Parse(www.downloadHandler.text))
            {
                UnityWebRequest www2 = UnityWebRequest.Get("");
                www2.SendWebRequest();
                while (!www2.isDone)
                {
                    Thread.Sleep(100);
                }
                File.WriteAllText(FilePath, www2.downloadHandler.text);
                ReadmeManager.InitReadmeList();
            }
        }
        public static string Updatelog;
        public static Action UpdateCall;
    }
}
