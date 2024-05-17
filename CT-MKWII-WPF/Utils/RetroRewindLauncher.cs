﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CT_MKWII_WPF.Utils;

public static class RetroRewindLauncher
{
    public static void PlayRetroRewind()
    {
        //if its not activated, activate it
        if (!DirectoryHandler.isRRActive())
        {
            DirectoryHandler.BackupRiivolution();
            DirectoryHandler.RetrieveRR();
        }
        //show a pop up if the user has not launched yet
        if (!SettingsUtils.HasRunNANDTutorial())
        {
            MessageBox.Show("Keep in mind, you need your NAND setup to play online. \nIf you have not set up your NAND yet you can NOT play online! \nIf you have already set up your NAND you may ignore this message", "NAND setup", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            SettingsUtils.setHasSeenNandPopUp(true);
        }
        //first clear the my-stuff folder
        //now we check for all the mods we want in the modconfig
        var mods = SettingsUtils.GetMods();
        if (mods == null || mods.Length == 0)
        {
            MessageBox.Show("You have no mods enabled. TEMP ERROR", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        //reverse the list, because mods that have the lowest priority go first
        //the reason we do this is, lets say file A from the lowest priority mod is in the highest priority mod, we want to overwrite it
        Array.Reverse(mods);
        var mystuffFolder = Path.Combine(SettingsUtils.GetLoadPathLocation(), "Riivolution", "RetroRewind6", "MyStuff");
        if (Directory.Exists(mystuffFolder))
        {
            Directory.Delete(mystuffFolder, true);
        }
        foreach (var mod in mods)
        {
            if (mod.IsEnabled)
            {
                DirectoryHandler.InstallMod(mod);
            }
        }
        
        string dolphinLocation = SettingsUtils.GetDolphinLocation();
        string gamePath = SettingsUtils.GetGameLocation();
        GenerateLaunchJSON();
        string launchJson = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "RR.json");

        if (!File.Exists(dolphinLocation) || !File.Exists(gamePath))
        {
            MessageBox.Show("Message patchzy with the following error: " + dolphinLocation + " " + gamePath, "Error", MessageBoxButton.OK, 
                MessageBoxImage.Error);
            return;
        }
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = dolphinLocation,
                Arguments = $"-e \"{launchJson}\"",
                UseShellExecute = false
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MessageBox.Show("Failed to start Dolphin Emulator. Please try again.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static void GenerateLaunchJSON()
    {
        string originalJSON = """
{
"base-file": "LINK TO ISO OR WBFS",
"display-name": "RR",
"riivolution": {
  "patches": [
    {
      "options": [
        {
          "choice": 1,
          "option-name": "Pack",
          "section-name": "Retro Rewind"
        },
        {
          "choice": 2,
          "option-name": "My Stuff",
          "section-name": "Retro Rewind"
        }
      ],
      "root": "LINK TO RIIVOLUTION FOLDER",
      "xml": "LINK TO RETRO REWIND XML FILE"
    }
  ]
},
"type": "dolphin-game-mod-descriptor",
"version": 1
}
""";

        // Replace the base-file with the game path
        string correctedGamePath = SettingsUtils.GetGameLocation().Replace(@"\", @"\/");
        originalJSON = originalJSON.Replace("LINK TO ISO OR WBFS", correctedGamePath);

        // Replace the link to appdata riivolution folder with the correct path
        string correctedRRPath = Path.Combine(SettingsUtils.GetLoadPathLocation(),"Riivolution");
        correctedRRPath = correctedRRPath.Replace(@"\", @"\/");
        originalJSON = originalJSON.Replace("LINK TO RIIVOLUTION FOLDER", correctedRRPath + @"\/");

        string correctedXMLPath = correctedRRPath + @"\/riivolution\/RetroRewind6.xml";
        originalJSON = originalJSON.Replace("LINK TO RETRO REWIND XML FILE", correctedXMLPath);

        // Write the json to the exe folder
        File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "RR.json"), originalJSON);
    }
}