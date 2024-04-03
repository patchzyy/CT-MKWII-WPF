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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            switch (MenuDropdown.SelectedIndex)
            {
                case 0:
                    MessageBox.Show("Welcome to Screen 1");
                    break;
                case 1:
                    MessageBox.Show("Welcome to Screen 2");
                    break;
                case 2:
                    MessageBox.Show("Welcome to Screen 3");
                    break;
                default:
                    MessageBox.Show("Please select an option");
                    break;
            }
        }
    }
}