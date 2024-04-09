using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Utils;

namespace CT_MKWII_WPF.Pages;

public partial class Riivolution : UserControl
{
public Riivolution()
    {
        InitializeComponent();
        UpdateActionButton();
    }

    private async void UpdateActionButton()
    {
        var dolphinInstalled = DolphinInstaller.IsDolphinInstalled();
        var RiivolutionInstalled = RiivolutionInstaller.IsRiivolutionInstalled();

        if (!dolphinInstalled)
        {
            ActionButton.Content = "Install Dolphin";
        }
        else if (!RiivolutionInstalled)
        {
            ActionButton.Content = "Install Riivolution";
        }
        else
        {
            ActionButton.Content = "Play Riivolution";
        }

        StatusText.Text = $"Dolphin: {(dolphinInstalled ? "Installed" : "Not Installed")}\n" +
                          $"RiiVolution: {(RiivolutionInstalled ? "Installed" : "Not Installed")}\n";
    }

    private async void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        var dolphinInstalled = DolphinInstaller.IsDolphinInstalled();
        var riivolutionInstalled = RiivolutionInstaller.IsRiivolutionInstalled();
        
        if (!dolphinInstalled)
        {
            DolphinInstaller.InstallDolphin();
        }
        else if (!riivolutionInstalled)
        {
            RiivolutionInstaller.InstallRiivolution();
        }
        else
        {
            MessageBox.Show("Starting Riivolution...");
        }
    }
}