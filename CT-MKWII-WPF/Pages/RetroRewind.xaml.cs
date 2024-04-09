using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Utils;
namespace CT_MKWII_WPF.Pages;

public partial class RetroRewind : UserControl
{
    public RetroRewind()
    {
        InitializeComponent();
        UpdateActionButton();
    }

    private async void UpdateActionButton()
    {
        var dolphinInstalled = DolphinInstaller.IsDolphinInstalled();
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        var retroRewindUpToDate = await RetroRewindInstaller.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        string latestRRVersion = await RetroRewindInstaller.GetLatestVersionString();

        if (!dolphinInstalled)
        {
            ActionButton.Content = "Install Dolphin";
        }
        else if (!retroRewindInstalled)
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

        if (!dolphinInstalled)
        {
            DolphinInstaller.InstallDolphin();
        }
        else if (!retroRewindInstalled)
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
}