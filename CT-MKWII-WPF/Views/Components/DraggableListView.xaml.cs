using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CT_MKWII_WPF.Views.ViewUtils;

namespace CT_MKWII_WPF.Views.Components;

public partial class DraggableListView : ListView
{

    private ListViewItem? _listViewItem;

    // Public property with a private setter and a public getter
    public ListViewItem? ContextMenuListViewItem
    {
        get { return _listViewItem; }
        private set { _listViewItem = value; }
    }
    
    public DraggableListView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!(View
                is GridView gridView)) return; 

        // here we create an empty column that will later be filled with the icon for each row
        // We create an empty column that will be filled in order to not mess up any other columns arrangement
        var gripColumn = new GridViewColumn {
            Width = 40,
            CellTemplate = new DataTemplate(),
            Header = new GridViewColumnHeader { Visibility = Visibility.Collapsed }
        };
        
        gridView.Columns.Insert(0, gripColumn);
    }
    
    
    public T? GetCurrentContextItem<T>() where T : class
    {
        if (_listViewItem == null) return null;
        return ItemContainerGenerator.ItemFromContainer(_listViewItem) as T;
    }
    
    private void DotsIcon_Click(object sender, RoutedEventArgs e)
    {
        _listViewItem = FindAncestor<ListViewItem>(e.OriginalSource)!;
        if (sender is FrameworkElement && ContextMenu != null)
            ContextMenu.IsOpen = true;
    }

    private void ListViewItem_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) => 
        _listViewItem = FindAncestor<ListViewItem>(e.OriginalSource);

    private void GripIcon_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("GripIcon_Click");
    }
}
