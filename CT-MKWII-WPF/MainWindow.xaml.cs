using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CT_MKWII_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string RetroRewindPath = @"%APPDATA%\Dolphin Emulator\Load\Riivolution\RetroRewind6\version.txt";
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ModSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modSelectionComboBox.SelectedIndex > 1) // Retro Rewind is selected
            {
                mainTabControl.Visibility = Visibility.Visible;
            }
        }

        private void VersionCheckButton_Click(object sender, RoutedEventArgs e)
        {
            string versionFilePath = Environment.ExpandEnvironmentVariables(RetroRewindPath);
            if (File.Exists(versionFilePath))
            {
                string version = File.ReadAllText(versionFilePath).Trim();
                versionCheckButton.Content = $"Current Version: {version}";
                actionButton.Content = version == "6.0.2" ? "Play" : "Update";
            }
            else
            {
                versionCheckButton.Content = "Not Installed";
                actionButton.Content = "Install";
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for install/update/play logic
            MessageBox.Show($"{actionButton.Content} functionality not implemented yet.");
        }
    }

}