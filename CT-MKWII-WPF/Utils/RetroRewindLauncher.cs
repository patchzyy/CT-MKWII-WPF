using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CT_MKWII_WPF.Utils;

public static class RetroRewindLauncher
{
    public static void PlayRetroRewind()
    {
        string dolphinLocation = LauncherUtils.GetDolphinLocation();
        string gamePath = LauncherUtils.GetGamePath();
        GenerateLaunchJSON();
        string launchJson = Path.Combine(Environment.CurrentDirectory, "RR.json");

        if (!File.Exists(dolphinLocation) || !File.Exists(gamePath))
        {
            MessageBox.Show("Please ensure both paths are correct and try again.", "Error", MessageBoxButton.OK,
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
          "choice": 0,
          "option-name": "My Stuff",
          "section-name": "Retro Rewind"
        }
      ],
      "root": "LINK TO APPDATA RIIVOLUTION FOLDER",
      "xml": "LINK TO RETRO REWIND XML FILE"
    }
  ]
},
"type": "dolphin-game-mod-descriptor",
"version": 1
}
""";

        // Replace the base-file with the game path
        string correctedGamePath = LauncherUtils.GetGamePath().Replace(@"\", @"\/");
        originalJSON = originalJSON.Replace("LINK TO ISO OR WBFS", correctedGamePath);

        // Replace the link to appdata riivolution folder with the correct path
        string correctedRRPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Dolphin Emulator", "Load", "Riivolution");
        correctedRRPath = correctedRRPath.Replace(@"\", @"\/");
        originalJSON = originalJSON.Replace("LINK TO APPDATA RIIVOLUTION FOLDER", correctedRRPath + @"\/");

        string correctedXMLPath = correctedRRPath + @"\/riivolution\/RetroRewind6.xml";
        originalJSON = originalJSON.Replace("LINK TO RETRO REWIND XML FILE", correctedXMLPath);

        // Write the json to the exe folder
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "RR.json"), originalJSON);
    }
}