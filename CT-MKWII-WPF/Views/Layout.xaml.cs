using System.Windows;
using System.Windows.Input;
using CT_MKWII_WPF.Views.Pages;

namespace CT_MKWII_WPF.Views;

public partial class Layout : Window
{
    public Layout()
    {
        InitializeComponent();
      
        Dashboard myPage = new Dashboard();
        ContentArea.Navigate(myPage);
    }
    
    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }
    
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}