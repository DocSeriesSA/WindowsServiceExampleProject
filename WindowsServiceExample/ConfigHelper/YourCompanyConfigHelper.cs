using Newtonsoft.Json;
using System;
using System.IO;

namespace WindowsServiceExample.ConfigHelper
{
    internal static class YourCompanyConfigHelper
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        private static readonly string folderPath = AppDomain.CurrentDomain.BaseDirectory;

        public static YourCompanyConfig LoadConfig()
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<YourCompanyConfig>(json);
            }
            catch (FileNotFoundException)
            {
                // If the config file has not been found, then build it empty. Config your paramaters and run the service again.

                var configObj = new YourCompanyConfig();
                SaveConfig(configObj);
                return configObj;
            }
        }

        public static void SaveConfig(YourCompanyConfig configObj)
        {
            Directory.CreateDirectory(folderPath);
            string json = JsonConvert.SerializeObject(configObj, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}