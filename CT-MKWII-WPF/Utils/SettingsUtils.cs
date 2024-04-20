using System.IO;
using System.Windows;
using Newtonsoft.Json;
using CT_MKWII_WPF.Pages;

namespace CT_MKWII_WPF.Utils
{
    public static class SettingsUtils
    {
        private static readonly string _configFilePath = "./config.json";
        private static Config _config;

        static SettingsUtils()
        {
            LoadConfigFromFile();
        }
        
        public static void SaveSettings(string dolphinPath, string gamePath, string userFolderPath)
        {
            // Update the _config object with the new values
            _config.DolphinLocation = dolphinPath;
            _config.GameLocation = gamePath;
            _config.UserFolderPath = userFolderPath;
            // Serialize the _config object to JSON and save it to the config file
            var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(_configFilePath, configJson);
        }
        public static void SaveSettings(string dolphinPath, string gamePath, string userFolderPath, bool hasRunNANDTutorial)
        {
            // Update the _config object with the new values
            _config.DolphinLocation = dolphinPath;
            _config.GameLocation = gamePath;
            _config.UserFolderPath = userFolderPath;
            _config.HasRunNANDTutorial = hasRunNANDTutorial;
            // Serialize the _config object to JSON and save it to the config file
            var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(_configFilePath, configJson);
        }

        private static void LoadConfigFromFile()
        {
            if (File.Exists(_configFilePath))
            {
                _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(_configFilePath));
            }
            else
            {
                _config = new Config();
            }
        }

        public static string GetConfigText()
        {
            return File.Exists(_configFilePath) ? File.ReadAllText(_configFilePath) : string.Empty;
        }
        
        public static void SetHasRunNANDTutorial(bool hasRunNANDTutorial)
        {
            _config.HasRunNANDTutorial = hasRunNANDTutorial;
            SaveSettings(_config.DolphinLocation, _config.GameLocation, _config.UserFolderPath, hasRunNANDTutorial);
        }

        public static string GetGameLocation() => _config.GameLocation;

        public static string GetDolphinLocation() => _config.DolphinLocation;

        public static string GetUserPathLocation() => _config.UserFolderPath;

        public static string GetLoadPathLocation()
        {
            return Path.Combine(_config.UserFolderPath, "Load");
        }

        public static bool HasRunNANDTutorial() => _config.HasRunNANDTutorial;

        public static bool IsConfigFileFinishedSettingUp()
        {
            if (_config == null || !Directory.Exists(_config.UserFolderPath) || !File.Exists(_config.DolphinLocation) || !File.Exists(_config.GameLocation))
            {
                MessageBox.Show("One or more required paths are missing. Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ChangeContent(new SettingsPage());
                return false;
            }

            return true;
        }

        public static string FindGFXFile()
        {

            var folderPath = GetUserPathLocation();
            var configFolder = Path.Combine(folderPath, "Config");
            var gfxFile = Path.Combine(configFolder, "GFX.ini");

            if (File.Exists(gfxFile))
            {
                return gfxFile;
            }

            MessageBox.Show($"Could not find GFX file, tried looking in {gfxFile}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return string.Empty;
        }

        private class Config
        {
            public string DolphinLocation { get; set; }
            public string GameLocation { get; set; }
            public string UserFolderPath { get; set; }
            public bool HasRunNANDTutorial { get; set; }
        }
    }
}