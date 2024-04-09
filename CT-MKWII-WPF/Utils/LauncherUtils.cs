using System.IO;

namespace CT_MKWII_WPF.Utils;

public class LauncherUtils
{
    public static string GetDolphinLocation()
    {
        if (File.Exists("config.txt"))
        {
            string[] settings = File.ReadAllLines("config.txt");
            return settings[0];
        }

        return string.Empty;
    }

    public static string GetGamePath()
    {
        if (File.Exists("config.txt"))
        {
            string[] settings = File.ReadAllLines("config.txt");
            return settings[1];
        }
 
        return string.Empty;
    }
}