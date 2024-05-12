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
using CT_MKWII_WPF.Utils;

namespace CT_MKWII_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ChangeContent(new SettingsPage());
        }
        

        public void ChangeContent(UserControl control)
        {
            ContentArea.Content = control;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SettingsPage();
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

            // Handle button clicks and switch content accordingly
        }

        private void MyStuff_Click(object sender, RoutedEventArgs e)
        {
            var MyStuffManager = new MyStuffManager();
            ChangeContent(MyStuffManager);
        }

        private void Game_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsUtils.doesConfigExist())
            {
                MessageBox.Show("Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ChangeContent(new SettingsPage());
                return;
            }
            var RetroRewindDolphinPage = new RetroRewindDolphin();
            ChangeContent(RetroRewindDolphinPage);
        }

        private void Extras_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature is not yet implemented.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        
    }
}