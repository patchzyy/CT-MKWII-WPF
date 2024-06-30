using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;

namespace CT_MKWII_WPF.Views.Pages
{
    public partial class ModsPage : Page
    {
        //public ObservableCollection<ModItem> Mods { get; set; }

        public ModsPage()
        {
            InitializeComponent();
            
            // // when setting in code, make sure you bind the Source like so in the XAML: ItemsSource="{Binding Mods}"
            
            //Mods = new ObservableCollection<ModItem>
            //{
            //    new ModItem { ModName = "Sample Mod 1",  IsEnabled=true },
            //    new ModItem { ModName = "Sample Mod 2",  IsEnabled=true },
            //    new ModItem { ModName = "Sample Mod 3",  IsEnabled=true }
            //};

            ModsListView.DataContext = this;
        }
        
        
    }
}