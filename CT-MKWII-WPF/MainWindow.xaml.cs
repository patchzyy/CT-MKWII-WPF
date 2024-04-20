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
using CT_MKWII_WPF.Pages;
using CT_MKWII_WPF.Pages.WiiMods;


namespace CT_MKWII_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void ChangeContent(UserControl control)
        {
            ContentArea.Content = control;
        }
        

        // private void Continue_Click(object sender, RoutedEventArgs e)
        // {
        //     //before accepting a click, check if config.txt is setup
        //     if (!File.Exists("./config.txt"))
        //     {
        //         MessageBox.Show("Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //         return;
        //     }
        //     switch (ModSelection.SelectedIndex)
        //     {
        //         case 0:
        //             ContentArea.Content = new RetroRewind();
        //             break;
        //         case 1:
        //             ContentArea.Content = new CTGP();
        //             break;
        //         case 2:
        //             ContentArea.Content = new Riivolution();
        //             break;
        //         case 3:
        //             ContentArea.Content = new PlaceholderMod();
        //             break;
        //         default:
        //             MessageBox.Show("Please select an option");
        //             break;
        //     }
        // }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SettingsPage();
        }

        private void ModSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModSelection == null || ConsoleSelection == null) return;
            if (ModSelection.SelectedIndex == 0 || ConsoleSelection.SelectedIndex == 0)
            {
                return;
            }
            SwitchContent();
        }

        private void ConsoleSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModSelection == null || ConsoleSelection == null) return;
            if (ModSelection.SelectedIndex == 0 || ConsoleSelection.SelectedIndex == 0)
            {
                return;
            }
            SwitchContent();
        }

        public void SwitchContent()    
        {

            if (!File.Exists("./config.json"))
            {
                MessageBox.Show("Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ChangeContent(new SettingsPage());
                return;
            }

            if (ConsoleSelection.SelectedIndex == 2)
            {
                SwitchonWii();
            }
            else if (ConsoleSelection.SelectedIndex == 1)
            {
                SwitchonDolphin();
            }
            
        }

        private void SwitchonWii()
        {
            int modIndexWii = ModSelection.SelectedIndex;

            switch (modIndexWii)
            {
                case 0:
                    break;
                case 1:
                    ContentArea.Content = new RetroRewindWii();
                    break;
                default:
                    break;
            }
        }

        private void SwitchonDolphin()
        {
            
            int modIndexDolphin = ModSelection.SelectedIndex;
            
            switch (modIndexDolphin)
            {
                case 0:
                    break;
                case 1:
                    ContentArea.Content = new RetroRewindDolphin();
                    break;
                case 2:
                    ContentArea.Content = new CTGP();
                    break;
                case 3:
                    ContentArea.Content = new RiivolutionDolphin();
                    break;
                case 4:
                    ContentArea.Content = new PlaceholderMod();
                    break;
                default:
                    MessageBox.Show("Please select a mod");
                    return;
            }
        }
        
        
    }
}