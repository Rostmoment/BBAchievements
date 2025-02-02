﻿using BepInEx;
using BepInEx.Bootstrap;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BBAchievements
{
    class Achievement
    {
        [JsonIgnore]
        public static Achievement[] All => cache.Values.ToArray();
        [JsonIgnore]
        public bool CheckForKeyCheats { get; private set; }
        [JsonIgnore]
        public bool CheckForUnityExplorer { get; private set; }
        [JsonIgnore]
        public bool CheckForPineDebug { get; private set; }
        [JsonIgnore]
        public string[] NotAllowedMods { get; private set; }
        [JsonIgnore]
        public string GUID { get; private set; }
        [JsonIgnore]
        private static Dictionary<string, Achievement> cache;
        [JsonIgnore]
        public bool unlocked;
        [JsonIgnore]
        public bool hide;
        [JsonIgnore]
        public string description;
        [JsonIgnore]
        public DateTime Date
        {
            get
            {
                if (date == null)
                    return DateTime.MinValue;
                if (date == "")
                    return DateTime.MinValue;
                return DateTime.Parse(date);
            }
            set
            {
                if (value == DateTime.MinValue)
                {
                    date = "";
                    return;
                }
                date = value.ToString();
            }
        }
        public string date;
        public string nameKey;
        private Achievement() { }
        public void Reset()
        {
            unlocked = false;
            Date = DateTime.MinValue;
            if (SaveFile.Instance.achievements.ContainsKey(PlayerFileManager.Instance.fileName))
            {
                SaveFile.Instance.achievements[PlayerFileManager.Instance.fileName].RemoveAll(x => x.nameKey == nameKey);
            }
            SaveFile.Instance.Save();
        }
        public void Unlock()
        {
            if (CheckForKeyCheats && Chainloader.PluginInfos.ContainsKey("alexbw145.baldiplus.pinedebug"))
                return;
            if (CheckForPineDebug && Chainloader.PluginInfos.ContainsKey("b99.tbb.baldiplus.keycheats"))
                return;
            if (CheckForUnityExplorer && Chainloader.PluginInfos.ContainsKey("com.sinai.unityexplorer"))
                return;
            if (Chainloader.PluginInfos.Keys.ToList().Exists(x => NotAllowedMods.Contains(x)))
                return;
            if (unlocked)
                return;
            HUDPatches.AddToQueue(this);
            unlocked = true;
            Date = DateTime.Now;
            if (!SaveFile.Instance.achievements.ContainsKey(PlayerFileManager.Instance.fileName))
                SaveFile.Instance.achievements.Add(PlayerFileManager.Instance.fileName, new List<Achievement>());
            SaveFile.Instance.achievements[PlayerFileManager.Instance.fileName].Add(this);
            SaveFile.Instance.Save();
        }
        public void Anticheat(bool checkForPineDebug = true, bool checkForCheatKeys = true, bool checkForUnityExplorer = true, params string[] notAllowedMod)
        {
            CheckForKeyCheats = checkForCheatKeys;
            CheckForPineDebug = checkForPineDebug;
            CheckForUnityExplorer = checkForUnityExplorer;
            NotAllowedMods = notAllowedMod;
        }
        public static Achievement Get(string name) => cache[name];
        public static bool Exists(string name) => cache.ContainsKey(name);
        public static Achievement Create(PluginInfo info, string name, string description, bool hide = false)
        {
            cache ??= new Dictionary<string, Achievement>();
            if (cache.TryGetValue(name, out Achievement achievement)) {
                BasePlugin.logger.LogWarning("Achievement " + name + " already exists");
                return achievement;
            }

            achievement = new Achievement()
            {
                description = description,
                nameKey = name,
                hide = hide,
                unlocked = false,
                GUID = info.Metadata.GUID,
                NotAllowedMods = new string[] { },
                CheckForKeyCheats = false,
                CheckForPineDebug = false,
                CheckForUnityExplorer = false,
            };
            if (!SaveFile.Instance.achievements.ContainsKey(PlayerFileManager.Instance.fileName))
                SaveFile.Instance.achievements.Add(PlayerFileManager.Instance.fileName, new List<Achievement>());
            if (SaveFile.Instance.achievements[PlayerFileManager.Instance.fileName].Exists(x => x.nameKey == name))
                achievement.unlocked = true;
            cache.Add(name, achievement);
            return achievement;
        }
    }
}
