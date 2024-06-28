﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components;

/*
EXAMPLES:
<local:SidebarRadioButton IconKind="{x:Static icon:PackIconMaterialKind.Account}" 
                 IconPack="Material"
                 Text="User Profile" 
                 IsChecked="True"/>
 */

public partial class SidebarRadioButton : UserControl
{
    public SidebarRadioButton()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty IconKindProperty =
        DependencyProperty.Register(nameof(IconKind), typeof(object), typeof(SidebarRadioButton), new PropertyMetadata(null));

    public object IconKind
    {
        get => GetValue(IconKindProperty);
        set => SetValue(IconKindProperty, value);
    }

    public static readonly DependencyProperty IconPackProperty =
        DependencyProperty.Register(nameof(IconPack), typeof(string), typeof(SidebarRadioButton), new PropertyMetadata(null));

    public string IconPack
    {
        get => (string)GetValue(IconPackProperty);
        set => SetValue(IconPackProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(SidebarRadioButton), new PropertyMetadata(string.Empty));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(SidebarRadioButton), new PropertyMetadata(false));

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
}