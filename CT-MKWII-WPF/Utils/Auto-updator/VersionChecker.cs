using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CT_MKWII_WPF.Pages;

namespace CT_MKWII_WPF.Utils.Auto_updator;

public static class VersionChecker
{
    private const string VersionFileURL = "https://raw.githubusercontent.com/patchzyy/CT-MKWII-WPF/main/version.txt";
    //this is the internal version of the program. On the github the version number is exposed.
    //I will only update the version.txt & this when there is a next stable release
    private const string CurrentVersion = "0.0.3";
    
    public static string GetVersionNumber()
    {
        return CurrentVersion;
    }

    public static void CheckForUpdates()
    {
        //extract the version number from the version.txt file
        using (var client = new WebClient())
        {
            try
            {
                var version = client.DownloadString(VersionFileURL).Trim();
                if (version != CurrentVersion)
                {
                    //ask the user if they want to update
                    var result = MessageBox.Show("A new version of CT-MKWII-WPF is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        //update the program
                        Update();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred while checking for updates. Please try again later. \n \nError: " + e.Message);
            }
        }
    }

    public static async Task Update()
    {
        //find the file location of the current program
        var currentLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //download latest version file
        var downloadurl = "https://github.com/patchzyy/CT-MKWII-WPF/releases/latest/download/CT-MKWII-WPF.exe";
        //appdata mkwii folder, if not found, create it
        var appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/CT-MKWII";
        if (!System.IO.Directory.Exists(appdataFolder))
        {
            System.IO.Directory.CreateDirectory(appdataFolder);
        }
        var tempFolder = appdataFolder + "/temp";
        if (!System.IO.Directory.Exists(tempFolder))
        {
            System.IO.Directory.CreateDirectory(tempFolder);
        }
        var updateFile = tempFolder + "/CT-MKWII-WPF.exe";
        
        //create a progress window
        var progressWindow = new ProgressWindow();
        progressWindow.Show();
        
        //download the update file
        await DownloadUpdateFile(downloadurl, updateFile, progressWindow);
        
        //rename the current exe file
        string backupExePath = Path.Combine(Path.GetDirectoryName(currentLocation), "CT-MKWII-WPF-old.exe");
        File.Move(currentLocation, backupExePath);
        
        //move the new exe file to the current location
        File.Copy(updateFile, currentLocation, overwrite:true);
        
        //clean up temp folder
        Directory.Delete(tempFolder, true);
        //restart the program
        Process.Start(currentLocation);
        Environment.Exit(0);
    }
    
    static async Task DownloadUpdateFile(string url, string filePath, ProgressWindow progressWindow)
    {
        using (HttpClient client = new HttpClient())
        {
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true))
                {
                    var downloadedBytes = 0;
                    var bufferSize = 8192;
                    var buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        var progress = totalBytes == -1 ? 0 : (int)((float)downloadedBytes / totalBytes * 100);
                        var status = $"Downloading... {downloadedBytes}/{totalBytes} bytes";

                        progressWindow.Dispatcher.Invoke(() =>
                        {
                            progressWindow.UpdateProgress(progress, status);
                        });
                    }
                }
            }
        }
    }
}