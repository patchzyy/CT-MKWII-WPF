using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Utils;
using CT_MKWII_WPF.Utils.DolphinHelpers;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CT_MKWII_WPF.Views.Pages;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
        FillUserPath();
        UpdateResolutionButtonsState();
    }
    
    private void LoadSettings()
    {
        if (!SettingsUtils.doesConfigExist()) return;
        if (!SettingsUtils.SetupCorrectly()) return;
        DolphinInputField.Text = SettingsUtils.GetDolphinLocation();
        MarioInputField.Text = SettingsUtils.GetGameLocation();
        UserPathInputField.Text = SettingsUtils.GetUserPathLocation();
    }

    private void UpdateResolutionButtonsState()
    {
        ResolutionStackPanel.IsEnabled = SettingsUtils.doesConfigExist() && SettingsUtils.SetupCorrectly();
        CheckBoxStackPanel.IsEnabled = SettingsUtils.doesConfigExist() && SettingsUtils.SetupCorrectly();
        if (CheckBoxStackPanel.IsEnabled)
        {
            VSyncButton.IsChecked = DolphinSettingHelper.ReadINISetting(SettingsUtils.FindGFXFile(), "Hardware", "VSync") == "True";
            //todo, yes yes move this to the backend, but also check for all 4 settings
            RecommendedButton.IsChecked = DolphinSettingHelper.ReadINISetting(SettingsUtils.FindGFXFile(), "Settings", "ShaderCompilationMode") == "2";
        }
        if (ResolutionStackPanel.IsEnabled)
        {
            int finalResolution;
            var resolution = DolphinSettingHelper.ReadINISetting(SettingsUtils.FindGFXFile(), "Settings", "InternalResolution");
            try
            {
                 finalResolution = int.Parse(resolution) - 1;
            }
            catch (Exception e)
            {
                 finalResolution = 0;
            }
            if (finalResolution >= 0 && finalResolution < ResolutionStackPanel.Children.Count)
            {
                ((RadioButton)ResolutionStackPanel.Children[finalResolution]).IsChecked = true;
            }
        }
        
    }
    
    

    private void FillUserPath()
    {
        if (DolphinInputField.Text != "") return;
        string folderPath = DolphinSettingHelper.GetDolphinFolderPath();
        if (!string.IsNullOrEmpty(folderPath))
        {
            UserPathInputField.Text = folderPath;
        }
    }


    private void DolphinBrowseClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
            Title = "Select Dolphin Emulator"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            DolphinInputField.Text = openFileDialog.FileName;
        }
    }

    private void MarioKartBrowseClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Game files (*.iso;*.wbfs;*.rvz)|*.iso;*.wbfs;*.rvz|All files (*.*)|*.*",
            Title = "Select Mario Kart Wii Game File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            MarioInputField.Text = openFileDialog.FileName;
        }
    }

    private void DolphinUserPathClick(object sender, RoutedEventArgs e)
    {
        string folderPath = DolphinSettingHelper.GetDolphinFolderPath();

        if (!string.IsNullOrEmpty(folderPath))
        {
            // Ask user if they want to use the automatically found folder
            var result = MessageBox.Show("**If you dont know what all of this means, just click yes :)**\n\nDolphin Emulator folder found. Would you like to use this folder?\n\n" + folderPath, "Dolphin Emulator Folder Found", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                UserPathInputField.Text = folderPath;
                return;
            }
        }
        else
        {
            MessageBox.Show("Dolphin Emulator folder not automatically found. Please try and find the folder manually, click 'help' for more information.", "Dolphin Emulator Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // If we end up here, this means either the path wasn't found, or the user wants to manually select the path
        // Make it so we have to select a folder, not an executable
        CommonOpenFileDialog dialog = new();
        dialog.IsFolderPicker = true;
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            UserPathInputField.Text = dialog.FileName;
        }
    }

    private void SaveButtonClick(object sender, RoutedEventArgs e)
    {
        string dolphinPath = DolphinInputField.Text;
        string gamePath = MarioInputField.Text;
        string userFolder = UserPathInputField.Text;
        if (!File.Exists(dolphinPath) || !File.Exists(gamePath) || !Directory.Exists(userFolder))
        {
            MessageBox.Show("Please ensure all paths are correct and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Save settings to a configuration file or system registry
        SettingsUtils.SaveSettings(dolphinPath, gamePath, userFolder);
        if(!SettingsUtils.SetupCorrectly())
        {
            MessageBox.Show("Please ensure all paths are correct and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            var SettingsWindow = (Layout)Application.Current.MainWindow;
            SettingsWindow.NavigateToPage(new SettingsPage());
            return;
        }
        UpdateResolutionButtonsState();
        
        MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void HomeButtonClick(object sender, RoutedEventArgs e)
    {
        var mainWindow = (Layout)Application.Current.MainWindow;
        mainWindow.NavigateToPage(new Dashboard());
    }

    private void VSyncPress(object sender, RoutedEventArgs e)
    {
        //todo: move this logic into the backend
        if (VSyncButton.IsChecked == true)
        {
            DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Hardware", "VSync", "True");
        }
        else
        {
            DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Hardware", "VSync", "False");
        }
    }

    private void RecommendedClick(object sender, RoutedEventArgs e)
    {
        if (RecommendedButton.IsChecked == true)
        {
            EnableReccommendedSettings();
        }
        else
        {
            DisableReccommendedSettings();
        }
    }
    
    //todo: everything under here should be moved to the backend
    private void EnableReccommendedSettings()
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "ShaderCompilationMode", "2");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings","WaitForShadersBeforeStarting", "True" );
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "MSAA", "0x00000002");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "SSAA", "False");
    }
    
    private void DisableReccommendedSettings()
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "ShaderCompilationMode", "0");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings","WaitForShadersBeforeStarting", "False" );
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "MSAA", "0x00000001");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "SSAA", "False");
    }

    private void UpdateResolution(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            var resolution = radioButton.Tag.ToString();
            DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "InternalResolution", resolution);
        }
    }
}
