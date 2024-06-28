using System;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Utils;

namespace CT_MKWII_WPF.Views.Pages;

public partial class Dashboard : Page
{
    public Dashboard()
    {
        InitializeComponent();
        UpdateActionButton();
    }

    private void PlayButtonClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    
    private async void UpdateActionButton()
    {
        //first check if the server is even enabled, if not then just give 1 pop up letting the user know
        var serverEnabled = await RetroRewindInstaller.IsServerEnabled();
        if (!serverEnabled)
        {
            MessageBox.Show("We can't connect to the RR servers. Check your internet connection\nThe servers might be down. Please check back later.\nLaunching in offline mode", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        var isConfigFileSetup = SettingsUtils.configCorrectAndExists();
        if (!isConfigFileSetup)
        {
            PlayButton.Text = "Settings";
            return;
        }
        var isUserFolderValid = DolphinInstaller.IsUserFolderValid();
        var retroRewindActive = DirectoryHandler.isRRActive();
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        bool installedButNotActive = retroRewindInstalled && !retroRewindActive;
        if (!SettingsUtils.IsConfigFileFinishedSettingUp())
        {
            PlayButton.Text = "Settings";
        }
        bool retroRewindUpToDate;
        if (serverEnabled)
        {
            retroRewindUpToDate = await RetroRewindInstaller.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        }
        else
        {
            retroRewindUpToDate = true;
        }
        if (!isUserFolderValid)
        {
            PlayButton.Text = "Settings";
        }
        else if (!retroRewindInstalled && !installedButNotActive)
        {
            PlayButton.Text = "Install Retro Rewind";
        }
        else if (!retroRewindUpToDate)
        {
            PlayButton.Text = "Update Retro Rewind";
        }
        else
        {
            PlayButton.Text = "Play Retro Rewind";
        }

    }
}
