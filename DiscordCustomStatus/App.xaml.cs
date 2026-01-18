using System.IO;
using System.Text.Json;
using System.Windows;

namespace DiscordCustomStatus
{
    public partial class App : Application
    {
        public static AppConfig Config { get; private set; }
        private const string configFilePath = "appsettings.json";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ReadConfig();
        }

        public static void ReadConfig()
        {
            var json = File.ReadAllText(configFilePath);
            Config = JsonSerializer.Deserialize<AppConfig>(json);
        } 

        public static void SaveConfig()
        {
            var json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
        }
    }

}
