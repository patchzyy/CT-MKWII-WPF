using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Utils;

public class RiivolutionInstaller
{
    public static bool IsRiivolutionInstalled()
    {
        var RiivolutionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution");
        return Directory.Exists(RiivolutionFolder);
    }
    
    static string RiivolutionURL = "https://aerialx.github.io/rvlution.net/riivolution.zip";
    
    public static void InstallRiivolution()
    {
        //since Riivolution has not had an update since 2013, i will NOT be implementing any update logic
        //check if riivolution exists
        if (IsRiivolutionInstalled())
        {
            MessageBox.Show("Riivolution is already installed");
            return;
        }
        //else create the folder
        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution"));
        //download the zip
        using (var client = new HttpClient())
        {
            var response = client.GetAsync(RiivolutionURL).Result;
            if (response.IsSuccessStatusCode)
            {
                using (var stream = response.Content.ReadAsStream())
                {
                    using (var fileStream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution", "riivolution.zip"), FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
            else
            {
                MessageBox.Show("Failed to download Riivolution");
                return;
            }
            //extract the zip
            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution", "riivolution.zip"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution"));
            //delete the zip
            File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution", "riivolution.zip"));
            //message box confirming the installation
            MessageBox.Show("Riivolution has been installed");
                
        }
        
        
        
        
    }

}