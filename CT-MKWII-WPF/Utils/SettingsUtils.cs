using System.IO;
using System.Windows;

namespace CT_MKWII_WPF.Utils;

public static class SettingsUtils
{
    public static string GetConfigText()
    {
        const string filepath = "./config.txt";
        if (File.Exists(filepath))
        {
            return File.ReadAllText(filepath);
        }
        // MessageBox.Show("Config file not found. Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return "";
    }
    public static string GetGameLocation()
    {
        if (GetConfigText() == "") return "";
        string[] lines = GetConfigText().Split('\n');
        return lines[1];
    }
    
    public static string GetDolphinLocation()
    {
        if (GetConfigText() == "") return "";
        string[] lines = GetConfigText().Split('\n');
        return lines[0];
    }
    
    public static string GetLoadPathLocation()
    {
        if (GetConfigText() == "") return "";
        string[] lines = GetConfigText().Split('\n');
        return lines[2];
    }
}