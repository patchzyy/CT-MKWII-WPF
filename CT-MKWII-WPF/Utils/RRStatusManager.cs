using System.Threading.Tasks;

namespace CT_MKWII_WPF.Utils;

public class RRStatusManager
{
    public enum ActionButtonStatus
    {
        NoServer,
        NoDolphin,
        ConfigNotFinished,
        noRR,
        noRRActive,
        RRnotReady,
        OutOfDate,
        UpToDate
    }

    public static async Task<ActionButtonStatus> GetCurrentStatus()
    {
        var serverEnabled = await RetroRewindInstaller.IsServerEnabled();
        if (!serverEnabled)
        {
            return ActionButtonStatus.NoServer;
        }
        var dolphinInstalled = DolphinInstaller.IsUserFolderValid();
        var retroRewindActive = DirectoryHandler.isRRActive();
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        if (!retroRewindInstalled) return ActionButtonStatus.noRR;
        bool retroRewindUpToDate;
        string latestRRVersion;
        bool installedButNotActive = retroRewindInstalled && !retroRewindActive;
        if (installedButNotActive) return ActionButtonStatus.noRRActive;
        if (!retroRewindActive) return ActionButtonStatus.RRnotReady;
        if (!dolphinInstalled) return ActionButtonStatus.NoDolphin;
        if (!SettingsUtils.IsConfigFileFinishedSettingUp()) return ActionButtonStatus.ConfigNotFinished;

        retroRewindUpToDate = await RetroRewindInstaller.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        if (!retroRewindUpToDate) return ActionButtonStatus.OutOfDate;
        latestRRVersion = await RetroRewindInstaller.GetLatestVersionString();
        return ActionButtonStatus.UpToDate;
        
    }
}