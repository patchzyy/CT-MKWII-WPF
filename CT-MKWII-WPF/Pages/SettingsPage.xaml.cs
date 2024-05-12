using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // For OpenFileDialog
using System.Diagnostics;
using CT_MKWII_WPF.Utils;
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
        string userFolder = DolphinUserFolderTextBox.Text;
        if (!File.Exists(dolphinPath) || !File.Exists(gamePath) || !Directory.Exists(userFolder))
        {
            MessageBox.Show("Please ensure all paths are correct and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Save settings to a configuration file or system registry
        SettingsUtils.SaveSettings(dolphinPath, gamePath, userFolder);

        MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        mainWindow.SwitchContent();
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
            Filter = "Game files (*.iso;*.wbfs;*.rvz)|*.iso;*.wbfs;*.rvz|All files (*.*)|*.*",
            Title = "Select Mario Kart Wii Game File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            GamePathTextBox.Text = openFileDialog.FileName;
        }
    }

    private void LoadSettings()
    {
        if (!SettingsUtils.doesConfigExist()) return;
        if (!SettingsUtils.SetupCorrectly()) return;
        DolphinPathTextBox.Text = SettingsUtils.GetDolphinLocation();
        GamePathTextBox.Text = SettingsUtils.GetGameLocation();
        DolphinUserFolderTextBox.Text = SettingsUtils.GetUserPathLocation();
    }

    private void BrowseDolphinAppDataButton_Click(object sender, RoutedEventArgs e)
    {
        //select the appdata dolphin folder
        //try to automatically find it
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator");
        if (Directory.Exists(appDataPath))
        {
            //ask user if they want to use this folder
            var result = MessageBox.Show("**If you dont know what all of this means, just click yes :)**\n\nDolphin Emulator folder found in AppData. Would you like to use this folder?", "Dolphin Emulator Folder Found", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DolphinUserFolderTextBox.Text = appDataPath;
                return;
            }
        }
        //path not found, check documents folder
        string documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Dolphin Emulator");
        if (Directory.Exists(documentsPath))
        {
            //ask user if they want to use this folder
            var result = MessageBox.Show("**If you dont know what all of this means, just click yes :)**\n\nDolphin Emulator folder found in Documents. Would you like to use this folder?", "Dolphin Emulator Folder Found", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DolphinUserFolderTextBox.Text = documentsPath;
                return;
            }
        }
        else
        {
            MessageBox.Show("Dolphin Emulator folder not automatically found. Please try and find the folder manually, click 'help' for more information.", "Dolphin Emulator Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        //if we end up here, this means either the path wasnt found, or  the user wants to manually select the path
        //make it so we have to select a folder, not an executable
        CommonOpenFileDialog dialog = new();
        dialog.IsFolderPicker = true;
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            DolphinUserFolderTextBox.Text = dialog.FileName;
        }

    }


    private void Dolphin_Path_Help_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("This is the path to the Dolphin Emulator executable. This is required to launch the game.", "Dolphin Path Help", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MKWII_Path_Help_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "This would be the location of your mario kart wii .iso .rvz or .wbfs file.\nI can't provide any sources of how to obtain one", "Mario Kart Wii Path Help", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void DolphinUserFolderHelp_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "This path should be found automatically once you click browse..., but if it doesnt follow these instructions\n\n1. Open your Dolphin emulator\n2. Click File \u2192 Open User Folder \n\nThis is required", "Dolphin Load Path Help", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e)
    {
        SaveButton_Click(sender, e);
        if (!SettingsUtils.doesConfigExist())
        {
            MessageBox.Show("Please set the paths in settings and save the settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var RetroRewindDolphinPage = new RetroRewindDolphin();
        var mw = (MainWindow)Application.Current.MainWindow;
        mw.ChangeContent(RetroRewindDolphinPage);
    }

}
