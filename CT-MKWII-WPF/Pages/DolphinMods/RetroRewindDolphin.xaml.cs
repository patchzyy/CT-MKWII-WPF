using System.Windows;
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
        Loaded += (sender, e) => GetCurrentVSyncStatus();
        Loaded += (sender, e) => UpdateCurrentUberShaderStatus();
        
        VSyncCheckbox.Checked += Enable_VSync;
        VSyncCheckbox.Unchecked += Disable_VSync;
        ReccommendedSettingsCheckBox.Checked += EnableReccommendedSettings;
        ReccommendedSettingsCheckBox.Unchecked += DisableReccommendedSettings;
    }
    
    private void EnableReccommendedSettings(object sender, RoutedEventArgs e)
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "ShaderCompilationMode", "2");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings","WaitForShadersBeforeStarting", "True" );
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "MSAA", "0x00000002");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "SSAA", "False");
    }
    
    private void DisableReccommendedSettings(object sender, RoutedEventArgs e)
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "ShaderCompilationMode", "0");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings","WaitForShadersBeforeStarting", "False" );
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "MSAA", "0x00000001");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "SSAA", "False");
    }
    
    private void UpdateCurrentUberShaderStatus()
    {
        var GFXFile = SettingsUtils.FindGFXFile();
        if (GFXFile == "")
        {
            ReccommendedSettingsCheckBox.IsChecked = false;
        }
        var UberShaders = DolphinSettingHelper.ReadINISetting(GFXFile, "Settings", "ShaderCompilationMode");
        if (UberShaders == "2")
        {
            ReccommendedSettingsCheckBox.IsChecked = true;
        }
        else
        {
            ReccommendedSettingsCheckBox.IsChecked = false;
        }
    }
    
    
    private void GetCurrentVSyncStatus()
    {
        var GFXFile = SettingsUtils.FindGFXFile();
        if (GFXFile == "")
        {
            MessageBox.Show("GFX file not found");
            VSyncCheckbox.IsChecked = false;
        }
        var VSync = DolphinSettingHelper.ReadINISetting(GFXFile,  "VSync");
        if (VSync.Trim() == "True")
        {
            VSyncCheckbox.IsChecked = true;
        }
        else
        {
            VSyncCheckbox.IsChecked = false;
        }
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
        //TODO: very dirty fix, please properly fix when there is no resolution or other settings set
        try
        {
            ResolutionDropDown.SelectedIndex = int.Parse(resolution) - 1;
        }
        catch
        {
            ResolutionDropDown.SelectedIndex = 0;
        }
    }

    private async void UpdateActionButton()
    {
        //first check if the server is even enabled, if not then just give 1 pop up letting the user know
        var serverEnabled = await RetroRewindInstaller.IsServerEnabled();
        if (!serverEnabled)
        {
            MessageBox.Show("We can't connect to the RR servers. Check your internet connection\nThe servers might be down. Please check back later.\nLaunching in offline mode", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        bool isUpdating = false;

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
            if (isUpdating)
            {
                MessageBox.Show("Already updating Retro Rewind", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            isUpdating = true;
            if (await RetroRewindInstaller.UpdateRR())
            {
                UpdateActionButton();
            }
            isUpdating = false;
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
        UpdateResolutionDropDown();
        return;
    }
    private void Enable_VSync(object sender, RoutedEventArgs e)
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Hardware", "VSync", "True");
        return;
    }
    
    private void Disable_VSync(object sender, RoutedEventArgs e)
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Hardware", "VSync", "False");
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