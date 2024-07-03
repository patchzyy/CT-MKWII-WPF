using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static CT_MKWII_WPF.Views.ViewUtils;

namespace CT_MKWII_WPF.Views.Components;

public partial class DraggableListView : ListView
{
    private ListViewItem? _listViewItem;
    //private Point _startPoint;
    
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
    
    public static readonly DependencyProperty ItemContextMenuProperty = DependencyProperty.Register(
        nameof(ItemContextMenu), typeof(ContextMenu), typeof(DraggableListView),
        new PropertyMetadata(null));

    public ContextMenu? ItemContextMenu
    {
        get => (ContextMenu)GetValue(ItemContextMenuProperty);
        set => SetValue(ItemContextMenuProperty, value);
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!(View is GridView gridView)) return; 

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
    
    private void ContextMenu_Click(object sender, RoutedEventArgs e)
    {
        _listViewItem = FindAncestor<ListViewItem>(e.OriginalSource)!;
        if (sender is FrameworkElement && ItemContextMenu != null)
            ItemContextMenu.IsOpen = true;
    }

    private void GripIcon_Hold(object sender, MouseButtonEventArgs e)
    {
        //_startPoint = e.GetPosition(this);
        ListViewItem listViewItem = FindAncestor<ListViewItem>(e.OriginalSource)!;
        listViewItem.Opacity = 0.4;
        var dragData = new DataObject("listViewItem", listViewItem);
        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
    }

    private void GripIcon_MouseMove(object sender, MouseEventArgs e)
    {
    }

    
    private void GripIcon_OnDrop(object sender, DragEventArgs e)
    {
        if(!(e.Data.GetData("listViewItem") is ListViewItem listViewItem)) return;
        listViewItem.Opacity = 1;
        Point dropPosition = e.GetPosition(this);
        int targetIndex = GetDropTargetIndex(dropPosition);
        targetIndex = Math.Clamp(targetIndex, 0, Items.Count - 1);

        Console.WriteLine(ItemsSource.GetType().BaseType);
        
        // the least worst solution now seems to be to create a interface something like DragableItem, then just check for that instead
        // then we can also add attribs to that Draggable item if that is needed
        
        if (!(ItemsSource is ObservableCollection<ModItem> itemList))
        {
            Console.WriteLine(ItemsSource.GetType().IsGenericType);
            Console.WriteLine("ERROR: This type of list is not usabled for a draggable container");
            return;
        }
   
        ModItem itemThing = (ItemContainerGenerator.ItemFromContainer(listViewItem) as ModItem)!;
        itemList.Remove(itemThing);
        itemList.Insert(targetIndex, itemThing);
        
        Console.WriteLine(targetIndex);
    }
    
    private int GetDropTargetIndex(Point dropPosition)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            ListViewItem listViewItem = (ListViewItem)ItemContainerGenerator.ContainerFromIndex(i);
            if (listViewItem != null)
            {
                Rect itemBounds = VisualTreeHelper.GetDescendantBounds(listViewItem);
                Point itemPosition = listViewItem.TransformToAncestor(this).Transform(new Point(0, 0));

                if (dropPosition.Y < itemPosition.Y + itemBounds.Height)
                {
                    return i;
                }
            }
        }

        return -1;
    }
}
