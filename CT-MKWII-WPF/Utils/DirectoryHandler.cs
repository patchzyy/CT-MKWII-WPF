using System.IO;

namespace CT_MKWII_WPF.Utils;

public class DirectoryHandler
{
    public static void BackupRR()
    {
        //check if RR is active in the load folder
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        if (Directory.Exists(Path.Combine(LoadPath, "Riivolution", "RetroRewind6")))
        {
            //move the entire Riivolution main folder to Riivolution-RR
            Directory.Move(Path.Combine(LoadPath, "Riivolution"), Path.Combine(LoadPath, "Riivolution-RR"));
        }
    }
    
    public static void RetrieveRR()
    {
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        if (Directory.Exists(Path.Combine(LoadPath, "Riivolution-RR")))
        {
            //move the entire Riivolution-RR folder to Riivolution
            Directory.Move(Path.Combine(LoadPath, "Riivolution-RR"), Path.Combine(LoadPath, "Riivolution"));
        }
    }
    
    public static void BackupRiivolution()
    {
        //first check if Riivolution is active in the load folder, but make sure we dont back up if RR is active
        
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        bool isRiiActive = isRiivolutionActive();
        if (isRiiActive)
        {
            //move the entire Riivolution folder to Riivolution-OG
            Directory.Move(Path.Combine(LoadPath, "Riivolution"), Path.Combine(LoadPath, "Riivolution-OG"));
        }

    }
    
    public static bool isRRActive()
    {
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        return Directory.Exists(Path.Combine(LoadPath, "Riivolution", "RetroRewind6"));
    }
    
    public static bool isRiivolutionActive()
    {
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        //only return true if there is NO RetroRewind6 folder in the Riivolution folder AND the Riivolution folder exists
        return !Directory.Exists(Path.Combine(LoadPath, "Riivolution", "RetroRewind6")) && Directory.Exists(Path.Combine(LoadPath, "Riivolution"));
        
    }
    
    public static void RetrieveRiivolution()
    {
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        if (Directory.Exists(Path.Combine(LoadPath, "Riivolution-OG")))
        {
            //move the entire Riivolution-OG folder to Riivolution
            Directory.Move(Path.Combine(LoadPath, "Riivolution-OG"), Path.Combine(LoadPath, "Riivolution"));
        }
    }
}