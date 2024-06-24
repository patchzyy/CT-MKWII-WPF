using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/*
 EXAMPLES:
 
    TODO:you can simply use it for a icon with a text like this:
 <local:IconLabel IconKind="{x:Static icon:PackIconGameIconsKind.CarWheel}" 
                 IconPack="GameIcons"
                 Text="Dig" 
                 Color="Green" 
                 FontSize="18"
                 IconSize="22"/>

    TODO: here is an example with a click event: (and a hover effect for that matter)
<local:IconLabel IconKind="{x:Static icon:PackIconMaterialKind.Account}" 
                 IconPack="Material"
                 Text="User Profile" 
                 Color="Blue" 
                 FontSize="20"
                 IconSize="24"
                HoverColor="Red"
                 Click="YourClickHandler"/>

   TODO: and here is an example with FontAwesome:
<local:IconLabel IconKind="{x:Static icon:PackIconFontAwesomeKind.SolidCoffee}" 
                 IconPack="FontAwesome"
                 Text="Coffee" 
                 Color="Brown" 
                 FontSize="16"
                 IconSize="20"/>
 */


namespace CT_MKWII_WPF.Views.Components
{
    public partial class IconLabel : UserControl
    {
        public IconLabel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Brush), typeof(IconLabel), 
            new PropertyMetadata(Brushes.Black, OnColorChanged));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty HoverColorProperty = DependencyProperty.Register(
            nameof(HoverColor), typeof(Brush), typeof(IconLabel),
            new PropertyMetadata(null));

        public Brush HoverColor
        {
            get => (Brush)GetValue(HoverColorProperty);
            set => SetValue(HoverColorProperty, value);
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconLabel iconLabel)
            {
                if (iconLabel.HoverColor == null)
                {
                    iconLabel.HoverColor = (Brush)e.NewValue;
                }
            }
        }
        
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
            nameof(IconSize), typeof(double), typeof(IconLabel),
            new PropertyMetadata(16.0));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(IconLabel), 
            new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register(
            nameof(IconKind), typeof(object), typeof(IconLabel), 
            new PropertyMetadata(null));

        public object IconKind
        {
            get => GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public static readonly DependencyProperty IconPackProperty = DependencyProperty.Register(
            nameof(IconPack), typeof(string), typeof(IconLabel), 
            new PropertyMetadata(null));

        public string IconPack
        {
            get => (string)GetValue(IconPackProperty);
            set => SetValue(IconPackProperty, value);
        }
        
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IconLabel));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }
}