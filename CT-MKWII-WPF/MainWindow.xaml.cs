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


namespace CT_MKWII_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            switch (ConsoleSelection.SelectedIndex)
            {
                case 0:
                    ContentArea.Content = new RetroRewind();
                    break;
                case 1:
                    ContentArea.Content = new CTGP();
                    break;
                case 2:
                    ContentArea.Content = new PlaceholderMod();
                    break;
                default:
                    MessageBox.Show("Please select an option");
                    break;
            }
        }
    }
}