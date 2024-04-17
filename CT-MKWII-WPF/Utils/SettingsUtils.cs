using System.IO;
using System.Windows;
using CT_MKWII_WPF.Pages;

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
    
    public static bool IsConfigFileFinishedSettingUp()
    {
        //go through every folder and/or file and check if they exist
        if (GetConfigText() == "") return false;
        if (!Directory.Exists(GetLoadPathLocation()))
        {
            MessageBox.Show("Load Path does not exist. Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //force user to settings page
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ChangeContent(new SettingsPage());

            return false;
        }
        if (!File.Exists(GetDolphinLocation()))
        {
            MessageBox.Show("Dolphin exe does not exist. Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ChangeContent(new SettingsPage());
            return false;
        }
        if (!File.Exists(GetGameLocation()))
        {
            MessageBox.Show("Game file does not exist. Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ChangeContent(new SettingsPage());
            return false;
        }
        return true;
        
    }
}