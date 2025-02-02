using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BBAchievements
{
    class SaveFile
    {
        [JsonIgnore]
        public string SavePath => Path.Combine(Application.persistentDataPath, "BBAchievements.json");
        [JsonIgnore]
        public static SaveFile Instance;

        public Dictionary<string, List<Achievement>> achievements;

        public SaveFile Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
                achievements = new Dictionary<string, List<Achievement>>();
            }
            return Instance;
        }

        public void Save()
        {
            File.WriteAllText(SavePath, JsonConvert.SerializeObject(SaveFile.Instance, Formatting.Indented));
        }
        public void Update()
        {
            Load();
            Save();
            Load();
        }
        public void Load()
        {
            if (File.Exists(SavePath))
            {
                JsonConvert.PopulateObject(File.ReadAllText(SavePath), SaveFile.Instance);
                return;
            }
            Save();
        }
    }
}
