using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // For OpenFileDialog
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs; // For Process

namespace CT_MKWII_WPF.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
    }
    
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        string dolphinPath = DolphinPathTextBox.Text;
        string gamePath = GamePathTextBox.Text;
        string loadFolder = DolphinLoadPathTextBox.Text;

        if (!File.Exists(dolphinPath) || !File.Exists(gamePath))
        {
            MessageBox.Show("Please ensure both paths are correct and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Save settings to a configuration file or system registry
        SaveSettings(dolphinPath, gamePath, loadFolder);

        MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BrowseDolphinButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
            Title = "Select Dolphin Emulator"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            DolphinPathTextBox.Text = openFileDialog.FileName;
        }
    }

    private void BrowseGameButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Game files (*.iso;*.wbfs)|*.iso;*.wbfs|All files (*.*)|*.*",
            Title = "Select Mario Kart Wii Game File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            GamePathTextBox.Text = openFileDialog.FileName;
        }
    }

    private void LoadSettings()
    {
        // try and find the config file
        if (!File.Exists("config.txt")) return;
        string[] settings = File.ReadAllLines("config.txt");
        if (settings.Length != 3) return;
        DolphinPathTextBox.Text = settings[0];
        GamePathTextBox.Text = settings[1];
        DolphinLoadPathTextBox.Text = settings[2];

    }

    private void SaveSettings(string dolphinPath, string gamePath, string loadPath)
    {
        //create a config file if it doesn't exist
        if (!File.Exists("config.txt"))
        {
            File.Create("config.txt").Close();
        }
        //write the paths to the config file
        File.WriteAllText("config.txt", dolphinPath + "\n" + gamePath + "\n" + loadPath);
        
    }

    private void BrowseDolphinAppDataButton_Click(object sender, RoutedEventArgs e)
    {
        //select the appdata dolphin folder
        //try to automatically find it
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load");
        if (Directory.Exists(appDataPath))
        {
            //ask user if they want to use this folder
            var result = MessageBox.Show("**If you dont know what all of this means, just click yes :)**\n\nDolphin Emulator folder found in AppData. Would you like to use this folder?", "Dolphin Emulator Folder Found", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DolphinLoadPathTextBox.Text = appDataPath;
                return;
            }
        }
        else
        {
            MessageBox.Show("Dolphin Emulator folder not found in AppData. Please try and find the folder manually, click 'help' for more information.", "Dolphin Emulator Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        //if we end up here, this means either the path wasnt found, or  the user wants to manually select the path
        //make it so we have to select a folder, not an executable
        CommonOpenFileDialog dialog = new();
        dialog.IsFolderPicker = true;
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            DolphinLoadPathTextBox.Text = dialog.FileName;
        }

    }


    private void Dolphin_Path_Help_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("This is the path to the Dolphin Emulator executable. This is required to launch the game.", "Dolphin Path Help", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MKWII_Path_Help_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "This would be the location of your mario kart wii .iso or .wbfs file.\nI can't provide any sources of how to obtain one, but the internet is your best friend :)", "Mario Kart Wii Path Help", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void AppDataPath_Help_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "This path should be found automatically, but if it doesnt follow these instructions\n\n1. Open your Dolphin emulator\n2. The Load Path can be found under Options \u2192 Configuration \u2192 Paths\n3. Look at the bottom, There should be one called 'Load Path' \n4. Copy the Load Path and paste it here\n\nThis is required", "Dolphin Load Path Help", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
