using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Utils;

public static class RetroRewindInstaller
{
    public static bool IsRetroRewindInstalled()
    {
        // var retroRewindFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution", "RetroRewind6");
        var loadPath = SettingsUtils.GetLoadPathLocation();
        var retroRewindFolder = Path.Combine(loadPath, "Riivolution", "RetroRewind6");
        return Directory.Exists(retroRewindFolder);
    }

    public static string CurrentRRVersion()
    {
        // var versionFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution", "RetroRewind6", "version.txt");
        var loadPath = SettingsUtils.GetLoadPathLocation();
        var versionFilePath = Path.Combine(loadPath, "Riivolution", "RetroRewind6", "version.txt");
        if (!File.Exists(versionFilePath)) return "Not Installed";
        
        return File.ReadAllText(versionFilePath);
    }

    public async static Task<bool> IsRRUpToDate(string currentVersion)
    {
        //make sure to ignore any spaces
        currentVersion = currentVersion.Trim();
        string latestVersion = await GetLatestVersionString();
        latestVersion = latestVersion.Trim();
        
        return currentVersion == latestVersion;
    }

    public static async Task<string> GetLatestVersionString()
    {
        const string FullTextURLText = "http://75.128.250.209:8000/RetroRewind/RetroRewindVersion.txt";
        try
        {
            using var httpClient = new HttpClient();
            string AllVersions = await httpClient.GetStringAsync(FullTextURLText);
            string[] lines = AllVersions.Split('\n');
            //now go to the last line split by spaces and grab index 0
            string latestVersion = lines[lines.Length - 1].Split(' ')[0];
            return latestVersion;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to check for updates: {ex.Message}");
            return "Failed to check for updates";
        }
        
    }

    public static async Task<bool> UpdateRR()
{
    string currentVersion = CurrentRRVersion();
    if (currentVersion == "Not Installed")
    {
        MessageBox.Show("Retro Rewind is not installed. Starting installation.");
        InstallRetroRewind();
        return true;
    }

    if (await IsRRUpToDate(currentVersion))
    {
        MessageBox.Show("Retro Rewind is up to date.");
        return true;
    }

    var allVersions = await GetAllVersionData();
    var updatesToApply = GetUpdatesToApply(currentVersion, allVersions);

    foreach (var update in updatesToApply)
    {
        MessageBox.Show($"Updating to version {update.Version}: {update.Description}");
        // MessageBox.Show($"Debug Info:\nVersion: {update.Version}\nURL: {update.Url}\nPath: {update.Path}\nDescription: {update.Description}");
        bool success = await DownloadAndApplyUpdate(update);
        if (!success)
        {
            MessageBox.Show("Failed to apply an update. Aborting.");
            return false;
        }
        UpdateVersionFile(update.Version);
        
    }
    MessageBox.Show("Update completed successfully.");
    return true;
}
    

private static void UpdateVersionFile(string newVersion)
{
    var loadPath = SettingsUtils.GetLoadPathLocation();
    var VersionFilePath = Path.Combine(loadPath, "Riivolution", "RetroRewind6", "version.txt");
    File.WriteAllText(VersionFilePath, newVersion);
}
private static async Task<List<(string Version, string Url, string Path, string Description)>> GetAllVersionData()
{
    List<(string Version, string Url, string Path, string Description)> versions = new List<(string Version, string Url, string Path, string Description)>();
    const string versionUrl = "http://75.128.250.209:8000/RetroRewind/RetroRewindVersion.txt";

    using var httpClient = new HttpClient();
    string allVersionsText = await httpClient.GetStringAsync(versionUrl);
    string[] lines = allVersionsText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (var line in lines)
    {
        string[] parts = line.Split(new[] { ' ' }, 4);
        if (parts.Length < 4) continue; // Skip if the line doesn't have enough parts.
        versions.Add((parts[0].Trim(), parts[1].Trim(), parts[2].Trim(), parts[3].Trim()));
    }

    return versions;
}

private static List<(string Version, string Url, string Path, string Description)> GetUpdatesToApply(string currentVersion, List<(string Version, string Url, string Path, string Description)> allVersions)
{
    var updatesToApply = new List<(string Version, string Url, string Path, string Description)>();
    bool startAdding = false;
    foreach (var version in allVersions)
    {
        if (version.Version == currentVersion) startAdding = true;
        else if (startAdding) updatesToApply.Add(version);
    }
    return updatesToApply;
}

private static async Task<bool> DownloadAndApplyUpdate((string Version, string Url, string Path, string Description) update)
{
    using var httpClient = new HttpClient();
    var tempZipPath = Path.GetTempFileName();

    try
    {
        var response = await httpClient.GetAsync(update.Url, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            MessageBox.Show($"Failed to download update: {update.Version}");
            return false;
        }

        using (var fs = new FileStream(tempZipPath, FileMode.Create))
        {
            await response.Content.CopyToAsync(fs);
        }

        // string extractionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution");
        var loadPath = SettingsUtils.GetLoadPathLocation();
        var extractionPath = Path.Combine(loadPath, "Riivolution");

        // Ensure the extraction path exists.
        Directory.CreateDirectory(extractionPath);

        // Extract the zip file.
        ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);

        MessageBox.Show($"Successfully applied update: {update.Version}");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred while applying the update: {ex.Message}");
        return false;
    }
    finally
    {
        // Clean up the temporary zip file.
        if (File.Exists(tempZipPath))
        {
            File.Delete(tempZipPath);
        }
    }

    return true;
}




    public static void InstallRetroRewind()
    {
        // Implement installation logic for Retro Rewind.
        MessageBox.Show("Retro Rewind installation logic not implemented yet.");
    }
    
    
}