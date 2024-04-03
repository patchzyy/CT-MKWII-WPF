using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Pages;

public partial class RetroRewind : UserControl
{
    
    public RetroRewind()
    {
        InitializeComponent();
        CheckDolphinInstallation(null, null);
        CheckRRInstallation(null, null);
        CheckRRUpdate(null, null);
        UpdateActionButton();
    }
    


    public static bool IsDolphinInstalled()
    {
        //check the appdata/roaming/Dolphin Emulator folder for the dolphin emulator
        if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Dolphin Emulator"))
        {
            MessageBox.Show("Dolphin Emulator is not installed. Please click install or Cancel to exit the program.");
            return false;
        }
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
        if (currentVersion != "3.0.1")
        {
            return false;
        }

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
        UpdateActionButton();
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
        UpdateActionButton();
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
        UpdateActionButton();
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
        UpdateActionButton();
    }

    private bool UpdateRR()
    {
        UpdateActionButton();
        return false;
    }
    
    private void UpdateActionButton()
    {
        if (!IsDolphinInstalled())
        {
            ActionButton.Content = "Install Dolphin";
        }
        else if (!IsRetroRewindInstalled())
        {
            ActionButton.Content = "Install Retro Rewind";
        }
        else if (!IsRRUpToDate(CurrentRRVersion()))
        {
            ActionButton.Content = "Update Retro Rewind";
        }
        else
        {
            ActionButton.Content = "Play Retro Rewind";
        }
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        if (!IsDolphinInstalled())
        {
            // Implement installation logic for Dolphin.
        }
        else if (!IsRetroRewindInstalled())
        {
            // Implement installation logic for Retro Rewind.
        }
        else if (!IsRRUpToDate(CurrentRRVersion()))
        {
            // Implement update logic for Retro Rewind.
            if (UpdateRR())
            {
                RRUpdateStatus.Text = "Retro Rewind has been updated successfully.";
                UpdateActionButton();
            }
            else
            {
                RRUpdateStatus.Text = "Failed to update Retro Rewind. Please try again.";
            }
        }
        else
        {
            PlayRetroRewind();
        }
    }

    public void PlayRetroRewind()
    {
        string dolphinLocation = GetDolphinLocation();
        string gamePath = GetGamePath();
        
        //check if the paths are correct
        if (!File.Exists(dolphinLocation) || !File.Exists(gamePath))
        {
            MessageBox.Show("Please ensure both paths are correct and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        //start the dolphin emulator with the game
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = dolphinLocation,
                Arguments = $"-e \"{gamePath}\"",
                UseShellExecute = false
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MessageBox.Show("Failed to start Dolphin Emulator. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }

    public string GetDolphinLocation()
    {
        //check the config.txt file for the dolphin location, top line is location
        if (File.Exists("config.txt"))
        {
            string[] settings = File.ReadAllLines("config.txt");
            return settings[0];
        }
        return "";
    }
    
    public string GetGamePath()
    {
        //check the config.txt file for the game location, second line is location
        if (File.Exists("config.txt"))
        {
            string[] settings = File.ReadAllLines("config.txt");
            return settings[1];
        }
        return "";
    }

}