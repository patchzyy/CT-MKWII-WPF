using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Pages;

public partial class RetroRewind : UserControl
{
    
    public RetroRewind()
    {
        InitializeComponent();
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
    
    

    private async Task<bool> UpdateRR()
    {
        //pop up a message box to confirm the update
        MessageBoxResult result = MessageBox.Show("Are you sure you want to update Retro Rewind?", "Update Retro Rewind", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.No)
        {
            return false;
        }
        const string latestVersionUrl = "https://raw.githubusercontent.com/patchzyy/CT-MKWII-WPF/main/Latest.txt";

        try
        {
            using (var httpClient = new HttpClient())
            {
                // Fetch the latest version string from GitHub
                string latestVersion = await httpClient.GetStringAsync(latestVersionUrl);
                latestVersion = latestVersion.Trim(); // Trim any whitespace

                // Version comparison (consider using Version class for more complex comparisons)
                    MessageBox.Show($"Updating to latest version {latestVersion}");
                    // Here you could trigger the actual update logic
                    UpdateActionButton();
                    return false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to check for updates: {ex.Message}");
            // Depending on your application's needs, you might want to return true or false here
            return false;
        }
        
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
        
        //also update the status text at the top left
        StatusText.Text = "Dolphin: " + (IsDolphinInstalled() ? "Installed" : "Not Installed") + "\n" +
                          "Retro Rewind: " + (IsRetroRewindInstalled() ? "Installed" : "Not Installed") + "\n" +
                          "Retro Rewind Version: " + CurrentRRVersion() + "\n" +
                          "Retro Rewind Up to Date: " + (IsRRUpToDate(CurrentRRVersion()) ? "Yes" : "No");
        
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
            if (UpdateRR() != null)
            {
                UpdateActionButton();
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
        //launch json is in exe folder
        GenerateLaunchJSON();
        string LaunchJSON = Path.Combine(Environment.CurrentDirectory, "RR.json");
        
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
                Arguments = $"-e \"{LaunchJSON}\"" ,
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

    public void GenerateLaunchJSON()
    {
        
        //keep in mind a few things, the json formatting of this file is so weird
        //paths should look like this "C:\/Users\/patchzy\/AppData\/Roaming\/Dolphin Emulator\/Load\/Riivolution\/"
        //so keep that in mind when replacing the paths, so the slashes should be escaped
        string OriginalJSON = """
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
        
        //replace the base-file with the game path
        string CorrectedGamePath = GetGamePath().Replace(@"\", @"\/");
        OriginalJSON = OriginalJSON.Replace("LINK TO ISO OR WBFS", CorrectedGamePath);
        
        //replace the link to appdata riivolution folder with the correct path
        string CorrectedRRPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Dolphin Emulator\Load\Riivolution\";
        CorrectedRRPath = CorrectedRRPath.Replace(@"\", @"\/");
        OriginalJSON = OriginalJSON.Replace("LINK TO APPDATA RIIVOLUTION FOLDER", CorrectedRRPath);
        
        string CorrectedXMLPath = CorrectedRRPath + @"\/riivolution\/RetroRewind6.xml";
        OriginalJSON = OriginalJSON.Replace("LINK TO RETRO REWIND XML FILE", CorrectedXMLPath);
        
        //write the json to the exe folder
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "RR.json"), OriginalJSON);
    }
    

}