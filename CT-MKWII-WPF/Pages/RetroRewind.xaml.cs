using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Pages;

public partial class RetroRewind : UserControl
{
    
    public RetroRewind()
    {
        InitializeComponent();
        //pop up window when the page loads to confirm it works
        if (!IsDolphinInstalled())
        {
            return;
        }
        if (!IsRetroRewindInstalled())
        {
            return;
        }
        if (!IsRRUpToDate(CurrentRRVersion()))
        {
            MessageBox.Show("Retro Rewind is not up to date. Please click install or Cancel to exit the program.");
        }
        
        
    }

    public static bool IsDolphinInstalled()
    {
        //check the appdata/roaming/Dolphin Emulator folder for the dolphin emulator
        if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Dolphin Emulator"))
        {
            MessageBox.Show("Dolphin Emulator is not installed. Please click install or Cancel to exit the program.");
            return false;
        }
        MessageBox.Show("Dolphin Emulator is installed.");
        return true;
    }

    public static bool IsRetroRewindInstalled()
    {
        //check Dolphin Emulator\Load\Riivolution\RetroRewind6 
        if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Dolphin Emulator\Load\Riivolution\RetroRewind6"))
        {
            MessageBox.Show("Retro Rewind is not installed. Please click install or Cancel to exit the program.");
            return false;
        }
        return true;
    }

    public static string CurrentRRVersion()
    {
        //check Dolphin Emulator\Load\Riivolution\RetroRewind6\version.txt
        if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Dolphin Emulator\Load\Riivolution\RetroRewind6\version.txt"))
        {
            MessageBox.Show("Retro Rewind is not installed. Please click install or Cancel to exit the program.");
            return "Not Installed";
        }
        return File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Dolphin Emulator\Load\Riivolution\RetroRewind6\version.txt");
    }

    public static bool IsRRUpToDate(string currentVersion)
    {
        //check the current version against the latest version
        if (currentVersion == "Not Installed")
        {
            MessageBox.Show("Retro Rewind is not installed. Please click install or Cancel to exit the program.");
            return false;
        }

        //if current version is lower then 6.0.0 then it is not up to date
        if (currentVersion.CompareTo("6.0.0") < 0)
        {
            return false;
        }

        MessageBox.Show("Retro Rewind is up to date.");
        return true;
    }
    
    private void CheckDolphinInstallation(object sender, RoutedEventArgs e)
    {
        if (IsDolphinInstalled())
        {
            DolphinStatus.Text = "Dolphin Emulator is installed.";
        }
        else
        {
            DolphinStatus.Text = "Dolphin Emulator is not installed. Please install.";
        }
    }

    private void CheckRRInstallation(object sender, RoutedEventArgs e)
    {
        if (IsRetroRewindInstalled())
        {
            RRStatus.Text = "Retro Rewind is installed.";
        }
        else
        {
            RRStatus.Text = "Retro Rewind is not installed. Please install.";
        }
    }

    private void CheckRRUpdate(object sender, RoutedEventArgs e)
    {
        string version = CurrentRRVersion();
        if (IsRRUpToDate(version))
        {
            RRUpdateStatus.Text = "Retro Rewind is up to date.";
        }
        else
        {
            RRUpdateStatus.Text = $"Retro Rewind is not up to date. Current version: {version}.";
        }
    }

    private void UpdateRRClick(object sender, RoutedEventArgs e)
    {
        // This function should trigger the update process for Retro Rewind.
        // Implement the UpdateRR method to actually perform the update.
        if (UpdateRR())
        {
            RRUpdateStatus.Text = "Retro Rewind has been updated successfully.";
        }
        else
        {
            RRUpdateStatus.Text = "Failed to update Retro Rewind. Please try again.";
        }
    }

    private bool UpdateRR()
    {
        return false;
    }
}