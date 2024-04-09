using System;
using System.IO;
using System.Windows;
using CT_MKWII_WPF.Pages;
using CT_MKWII_WPF.Utils;

public static class DolphinInstaller
{
    public static bool IsDolphinInstalled()
    {
        // var dolphinFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator");
        var loadPath = SettingsUtils.GetLoadPathLocation();
        //go back 1 folder
        var dolphinFolder = Path.Combine(loadPath, "..");
        return Directory.Exists(dolphinFolder);
    }

    public static void InstallDolphin()
    {
        // Implement installation logic for Dolphin.
        MessageBox.Show("Dolphin installation logic not implemented yet.");
    }
}