using Newtonsoft.Json;
using System;
using System.IO;

namespace NeqPass.GUI.Types
{
    class Settings
    {
        public const string SettingsFileName = "settings.json";

        private static readonly object _lockObj = new object();

        public bool OpenInMinimized { get; set; }
        public bool SavePath { get; set; }
        public string SavedPath { get; set; }
        
        public bool AutoBlock { get; set; }
        public TimeSpan AutoBlockDuration { get; set; }

        public static Settings Current;

        public static Settings LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                var settings = new Settings
                {
                    OpenInMinimized = false,
                    SavePath = false,
                    SavedPath = "",
                    AutoBlock = false,
                    AutoBlockDuration = TimeSpan.MaxValue
                };

                lock (_lockObj)
                {
                    File.WriteAllText(fileName, JsonConvert.SerializeObject(settings));
                }

                return settings;
            }
            else
            {
                lock (_lockObj)
                {
                    return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(fileName));
                }
            }
        }

        public void Save(string fileName)
        {
            lock (_lockObj)
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(this));
            }
        }


    }
}
