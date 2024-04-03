using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // For OpenFileDialog
using System.Diagnostics; // For Process

namespace CT_MKWII_WPF.Pages;

public partial class Settings : UserControl
{
    public Settings()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        string dolphinPath = DolphinPathTextBox.Text;
        string gamePath = GamePathTextBox.Text;

        if (!File.Exists(dolphinPath) || !File.Exists(gamePath))
        {
            MessageBox.Show("Please ensure both paths are correct and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Save settings to a configuration file or system registry
        SaveSettings(dolphinPath, gamePath);

        MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BrowseDolphinButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
            Title = "Select Dolphin Emulator"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            DolphinPathTextBox.Text = openFileDialog.FileName;
        }
    }

    private void BrowseGameButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Game files (*.iso;*.wbfs)|*.iso;*.wbfs|All files (*.*)|*.*",
            Title = "Select Mario Kart Wii Game File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            GamePathTextBox.Text = openFileDialog.FileName;
        }
    }

    private void LoadSettings()
    {
        // try and find the config file
        if (File.Exists("config.txt"))
        {
            string[] settings = File.ReadAllLines("config.txt");
            if (settings.Length == 2)
            {
                DolphinPathTextBox.Text = settings[0];
                GamePathTextBox.Text = settings[1];
            }
        }
        
    }

    private void SaveSettings(string dolphinPath, string gamePath)
    {
        //create a config file if it doesn't exist
        if (!File.Exists("config.txt"))
        {
            File.Create("config.txt").Close();
        }
        //write the paths to the config file
        File.WriteAllText("config.txt", dolphinPath + "\n" + gamePath);
        
    }
}
