using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

public static class RetroRewindInstaller
{
    public static bool IsRetroRewindInstalled()
    {
        var retroRewindFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution", "RetroRewind6");
        return Directory.Exists(retroRewindFolder);
    }

    public static string CurrentRRVersion()
    {
        var versionFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution", "RetroRewind6", "version.txt");
        if (!File.Exists(versionFilePath))
        {
            MessageBox.Show("Retro Rewind is not installed. Please click install or Cancel to exit the program.");
            return "Not Installed";
        }
        return File.ReadAllText(versionFilePath);
    }

    public static bool IsRRUpToDate(string currentVersion)
    {
        if (currentVersion == "Not Installed")
        {
            MessageBox.Show("Retro Rewind is not installed. Please click install or Cancel to exit the program.");
            return false;
        }

        // If the current version is lower than 3.0.1, it is not up-to-date
        return currentVersion == "3.0.1";
    }

    public static async Task<bool> UpdateRR()
    {
        const string latestVersionUrl = "https://raw.githubusercontent.com/patchzyy/CT-MKWII-WPF/main/Latest.txt";

        MessageBoxResult result = MessageBox.Show("Are you sure you want to update Retro Rewind?", "Update Retro Rewind", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.No)
        {
            return false;
        }

        try
        {
            using (var httpClient = new HttpClient())
            {
                string latestVersion = await httpClient.GetStringAsync(latestVersionUrl);
                latestVersion = latestVersion.Trim();

                MessageBox.Show($"Updating to latest version {latestVersion}");
                // Here you could trigger the actual update logic
                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to check for updates: {ex.Message}");
            return false;
        }
    }

    public static void InstallRetroRewind()
    {
        // Implement installation logic for Retro Rewind.
        MessageBox.Show("Retro Rewind installation logic not implemented yet.");
    }
}