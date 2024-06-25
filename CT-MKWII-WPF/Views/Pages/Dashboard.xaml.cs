using System;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages;

public partial class Dashboard : Page
{
    public Dashboard()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Button clicked!");
    }
}