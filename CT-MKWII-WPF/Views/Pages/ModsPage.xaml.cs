using System;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Pages
{
    public partial class ModsPage : Page
    {
        public ObservableCollection<ModItem> Mods { get; set; }

        public ModsPage()
        {
            InitializeComponent();
            
            Mods = new ObservableCollection<ModItem>
            {
                new ModItem { ModName = "Sample Mod 1",  IsEnabled=true },
                new ModItem { ModName = "Sample Mod 2",  IsEnabled=true },
                new ModItem { ModName = "Sample Mod 3",  IsEnabled=false },
                new ModItem { ModName = "Sample Mod 4",  IsEnabled=false },
                new ModItem { ModName = "Sample Mod 5",  IsEnabled=true },
                
                new ModItem { ModName = "Sample Mod 1",  IsEnabled=true },
                new ModItem { ModName = "Sample Mod 2",  IsEnabled=true },
                new ModItem { ModName = "Sample Mod 3",  IsEnabled=false },
                new ModItem { ModName = "Sample Mod 4",  IsEnabled=false },
                new ModItem { ModName = "Sample Mod 5",  IsEnabled=true },
                
                new ModItem { ModName = "Sample Mod 1",  IsEnabled=true },
                new ModItem { ModName = "Sample Mod 2",  IsEnabled=true },
                new ModItem { ModName = "Sample Mod 3",  IsEnabled=false },
                new ModItem { ModName = "Sample Mod 4",  IsEnabled=false },
                new ModItem { ModName = "Sample Mod 5",  IsEnabled=true },
                
                new ModItem { ModName = "Sample Mod 1",  IsEnabled=true },
                new ModItem { ModName = "Sample Mod 2",  IsEnabled=true },
                new ModItem { ModName = "Sample Mod 3",  IsEnabled=false },
                new ModItem { ModName = "Sample Mod 4",  IsEnabled=false },
                new ModItem { ModName = "Sample Mod 5",  IsEnabled=true },
            };

            ModsListView.DataContext = this;
        }


        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var mySelectedMod = ModsListView.GetCurrentContextItem<ModItem>();
            Console.WriteLine( mySelectedMod );
        }

        private void ModsListView_OnOnItemClick(object sender, MouseButtonEventArgs e, ListViewItem clickedItem)
        {
            var mySelectedMod = (ModItem)clickedItem.DataContext;
            Console.WriteLine( mySelectedMod );
        }

        private void ModsListView_OnOnItemsReorder(ListViewItem movedItem, int newIndex)
        {
            var mySelectedMod = (ModItem)movedItem.DataContext;
            Console.WriteLine( $"({mySelectedMod}) moved to {newIndex}" );
        }
    }
}