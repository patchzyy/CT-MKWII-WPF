﻿using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Utils;
using CT_MKWII_WPF.Utils.DolphinHelpers;

namespace CT_MKWII_WPF.Pages;

public partial class RetroRewindDolphin : UserControl
{
    public RetroRewindDolphin()
    {
        InitializeComponent();
        UpdateActionButton();
        UpdateResolutionDropDown();
        Loaded += (sender, e) => ResolutionDropDown.SelectionChanged += Change_Resolution;
    }

    private void UpdateResolutionDropDown()
    {
        //read the setting from the GFX file
        var GFXFile = SettingsUtils.FindGFXFile();
        if (GFXFile == "")
        {
            return;
        }
        var resolution = DolphinSettingHelper.ReadINISetting(GFXFile, "Settings", "InternalResolution");
        ResolutionDropDown.SelectedIndex = int.Parse(resolution) - 1;
        
    }

    private async void UpdateActionButton()
    {
        //first check if the server is even enabled, if not then just give 1 pop up letting the user know
        var serverEnabled = await RetroRewindInstaller.IsServerEnabled();
        if (!serverEnabled)
        {
            MessageBox.Show("We can't connect to the RR servers.\nPlease check back later.\n Launching in offline mode", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        var dolphinInstalled = DolphinInstaller.IsDolphinInstalled();
        var retroRewindActive = DirectoryHandler.isRRActive();
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        bool installedButNotActive = retroRewindInstalled && !retroRewindActive;


        if (!SettingsUtils.IsConfigFileFinishedSettingUp())
        {
            return;
        }
        bool retroRewindUpToDate;
        string latestRRVersion;
        if (serverEnabled)
        {
            retroRewindUpToDate = await RetroRewindInstaller.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
            latestRRVersion = await RetroRewindInstaller.GetLatestVersionString();
        }
        else
        {
            retroRewindUpToDate = true;
            latestRRVersion = "N/A";
        }
        string currentStatus = CurrentStatus.Text;

        if (!dolphinInstalled)
        {
            ActionButton.Content = "Install Dolphin";
        }
        else if (!retroRewindInstalled && !installedButNotActive)
        {
            ActionButton.Content = "Install Retro Rewind";
        }
        else if (!retroRewindUpToDate)
        {
            ActionButton.Content = "Update Retro Rewind";
        }
        else
        {
            ActionButton.Content = "Play Retro Rewind";
        }

        StatusText.Text = $"Dolphin: {(dolphinInstalled ? "Installed" : "Not Installed")}\n" +
                          $"Retro Rewind: {(retroRewindInstalled ? "Installed" : "Not Installed")}\n" +
                          $"Retro Rewind Version: {RetroRewindInstaller.CurrentRRVersion()}\n" +
                          $"Retro Rewind Up to Date: {(retroRewindUpToDate ? "Yes\n" : "No\n")}" +
                          $"Latest RR Version: {latestRRVersion}";
    }

    private async void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        var dolphinInstalled = DolphinInstaller.IsDolphinInstalled();
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        var retroRewindUpToDate = await RetroRewindInstaller.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        var retroRewindActive = DirectoryHandler.isRRActive();
        bool installedButNotActive = retroRewindInstalled && !retroRewindActive;

        if (!dolphinInstalled)
        {
            DolphinInstaller.InstallDolphin();
        }
        
        else if (!retroRewindInstalled && !installedButNotActive)
        {
            RetroRewindInstaller.InstallRetroRewind();
        }
        else if (!retroRewindUpToDate)
        {
            if (await RetroRewindInstaller.UpdateRR())
            {
                UpdateActionButton();
            }
        }
        else
        {
            RetroRewindLauncher.PlayRetroRewind();
        }
    }

    private void Change_Resolution(object sender, SelectionChangedEventArgs e)
    {
        int Resolution = ResolutionDropDown.SelectedIndex + 1;
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "InternalResolution", Resolution.ToString());
        // MessageBox.Show("Resolution has been changed!");
        UpdateResolutionDropDown();
        return;
    }

    private void SetOptimalDolphinSettings(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void TriggerNANDSetup(object sender, RoutedEventArgs e)
    {
        NANDTutorialUtils.RunNANDTutorial();
        
    }
}