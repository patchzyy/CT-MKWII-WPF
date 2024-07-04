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
        var height = listViewItem.ActualHeight;
        listViewItem.Style = (Style)FindResource("DraggedItemStyle");
        listViewItem.Height = height + 3; 
        // i am probably forgetting something, but the height is 3 pix to short every time, idk why
        var dragData = new DataObject("listViewItem", listViewItem);
        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
    }

    private void GripIcon_MouseMove(object sender, MouseEventArgs e)
    {
    }

    
    private void GripIcon_OnDrop(object sender, DragEventArgs e)
    {
        if(!(e.Data.GetData("listViewItem") is ListViewItem listViewItem)) return;
        Point dropPosition = e.GetPosition(this);
        int targetIndex = GetDropTargetIndex(dropPosition);
        targetIndex = Math.Clamp(targetIndex, 0, Items.Count - 1);
        
        var itemObject = ItemContainerGenerator.ItemFromContainer(listViewItem)!;
        var itemType = itemObject.GetType();
        var genericCollectionType = typeof(ObservableCollection<>).MakeGenericType(itemType);
        var removeMethod = genericCollectionType.GetMethod("Remove", new[] { itemType });
        var insertMethod = genericCollectionType.GetMethod("Insert", new[] { typeof(int), itemType });

        if (removeMethod != null && insertMethod != null)
        {
            removeMethod.Invoke(ItemsSource, new[] { itemObject });
            insertMethod.Invoke(ItemsSource, new object[] { targetIndex, itemObject });
        } 
        else Console.WriteLine("It seems this collection type does not support in-place reordering");
     
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