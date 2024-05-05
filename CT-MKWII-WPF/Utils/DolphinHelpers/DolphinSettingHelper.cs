using System.IO;
using System.Windows;

namespace CT_MKWII_WPF.Utils.DolphinHelpers;

public class DolphinSettingHelper
{
        public static string ReadINISetting(string FileLocation, string Section, string SettingToRead)
    {
        if (!File.Exists(FileLocation))
        {
            MessageBox.Show("Something went wrong, INI file could not be read, Message Patchzy with the following error: " + FileLocation, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        //read through every line to find the section
        var lines = File.ReadAllLines(FileLocation);
        var sectionFound = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == $"[{Section}]")
            {
                sectionFound = true;
                break;
            }
        }
        if (!sectionFound)
        {
            // MessageBox.Show("Could not find section in INI file, Message Patchzy with the following error: " + Section, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return "";
        }
        //now we know the section exists, we need to find the setting
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(SettingToRead))
            {
                //we found the setting, now we need to return the value
                var setting = lines[i].Split("=");
                return setting[1].Trim();
            }
        }
        // MessageBox.Show("Could not find setting in INI file, Message Patchzy with the following error: " + SettingToRead, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return "";
    }
    public static void ChangeINISettings(string FileLocation, string Section, string SettingToChange, string Value)
    {
        if (!File.Exists(FileLocation))
        {
            MessageBox.Show("Something went wrong, INI file could not be found, Message Patchzy with the following error: " + FileLocation, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        //now we can assume file exists, so we good to go
        var lines = File.ReadAllLines(FileLocation);
        //first we need to see if the section already exists
        var sectionExists = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == $"[{Section}]")
            {
                sectionExists = true;
                break;
            }
        }
        if (!sectionExists)
        {
            //add the section to the end of the file
            string toAdd = $"[{Section}]";
            File.AppendAllText(FileLocation, toAdd + "\n");
        }
        //now we know for sure the section exists, but before continuing, we also need to check if the section doesnt already contain the setting to change
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(SettingToChange))
            {
                //we found the setting, now we need to change it
                lines[i] = $"{SettingToChange} = {Value}";
                File.WriteAllLines(FileLocation, lines);
                return;
            }
        }
        //if we reach here, then the setting does not exist, so we need to add it to the correct section
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == $"[{Section}]")
            {
                //we found the section, now we need to add the setting
                string toAdd = $"{SettingToChange} = {Value}";
                File.AppendAllText(FileLocation, toAdd + "\n");
                return;
            }
        }
        return;
    }
}