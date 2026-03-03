using System.IO;
using System.Text.Json;

namespace DiscordCustomStatus.AppConfig
{
    public static class AppConfigHelper
    {
        private const string configFilePath = "appconfig.json";
        private static AppConfig _config = null;
        public static AppConfig Config
        {
            get
            {
                if (_config == null)
                {
                    InitConfig();
                }
                return _config;
            }
        }

        public static void SaveConfig()
        {
            CreateIfNotExist();

            var json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
        }

        private static void InitConfig()
        {
            CreateIfNotExist();

            var json = File.ReadAllText(configFilePath);
            try
            {
                _config = JsonSerializer.Deserialize<AppConfig>(json);
            }
            catch
            {
                _config = new AppConfig
                {
                    DcsConfigs = []
                };
            }
        }

        private static void CreateIfNotExist()
        {
            if (!File.Exists(configFilePath))
            {
                File.Create(configFilePath).Dispose();
            }
        }
    }
}
