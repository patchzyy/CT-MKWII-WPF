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

    private void GotoSettingsPage()
    {
        var layout = (Layout) Application.Current.MainWindow;
        layout.NavigateToPage(new SettingsPage());
    }


    private async void PlayButtonClick(object sender, RoutedEventArgs e)
    {
        RRStatusManager.ActionButtonStatus status = await RRStatusManager.GetCurrentStatus();
        switch (status)
        {
            case RRStatusManager.ActionButtonStatus.NoServer: 
                GotoSettingsPage();
                break;
            case RRStatusManager.ActionButtonStatus.NoDolphin:
                GotoSettingsPage();
                break;
            case RRStatusManager.ActionButtonStatus.ConfigNotFinished:
                GotoSettingsPage();
                break;
            case RRStatusManager.ActionButtonStatus.noRR:
                await RetroRewindInstaller.InstallRetroRewind();
                UpdateActionButton();
                break;
            case RRStatusManager.ActionButtonStatus.noRRActive:
                //this is here for future use,
                //right now there is no de-activation, but if we want multiple mods this might be handy
                MessageBox.Show("Activate Retro Rewind");
                break;
            case RRStatusManager.ActionButtonStatus.RRnotReady:
                // RetroRewindInstaller.ActivateRetroRewind();
                break;
            case RRStatusManager.ActionButtonStatus.OutOfDate:
                await RetroRewindInstaller.UpdateRR();
                break;
            case RRStatusManager.ActionButtonStatus.UpToDate:
                RetroRewindLauncher.PlayRetroRewind();
                break;
        }
        UpdateActionButton();
    }
    
    private async void UpdateActionButton()
    {
        RRStatusManager.ActionButtonStatus status = await RRStatusManager.GetCurrentStatus();
        switch (status)
        {
            case RRStatusManager.ActionButtonStatus.NoServer:
                ActionButton.Text = "No Server";
                ActionButton.IsEnabled = true;
                ActionButton.Variant = "Secondary";
                break;
            case RRStatusManager.ActionButtonStatus.NoDolphin:
                ActionButton.Text = "Settings";
                ActionButton.IsEnabled = true;
                ActionButton.Variant = "Secondary";
                break;
            case RRStatusManager.ActionButtonStatus.ConfigNotFinished:
                ActionButton.Text = "Config Not Finished";
                ActionButton.IsEnabled = true;
                ActionButton.Variant = "Secondary";
                break;
            case RRStatusManager.ActionButtonStatus.noRR:
                ActionButton.Text = "Install Retro Rewind";
                ActionButton.IsEnabled = true;
                ActionButton.Variant = "Secondary";
                break;
            case RRStatusManager.ActionButtonStatus.noRRActive:
                //this is here for future use,
                //right now there is no de-activation, but if we want multiple mods this might be handy
                ActionButton.Text = "Activate Retro Rewind";
                ActionButton.Variant = "Secondary";
                ActionButton.IsEnabled = true;
                break;
            case RRStatusManager.ActionButtonStatus.RRnotReady:
                ActionButton.Text = "Activate Retro Rewind";
                ActionButton.Variant = "Secondary";
                ActionButton.IsEnabled = true;
                break;
            case RRStatusManager.ActionButtonStatus.OutOfDate:
                ActionButton.Text = "Update Retro Rewind";
                ActionButton.Variant = "Secondary";
                ActionButton.IsEnabled = true;
                break;
            case RRStatusManager.ActionButtonStatus.UpToDate:
                ActionButton.Text = "Play Retro Rewind";
                ActionButton.Variant = "Primary";
                ActionButton.IsEnabled = true;
                break;
        }

    }
}
