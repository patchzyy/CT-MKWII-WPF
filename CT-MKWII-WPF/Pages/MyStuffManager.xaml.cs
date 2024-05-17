using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CT_MKWII_WPF.Utils;
using Microsoft.Win32;

namespace CT_MKWII_WPF.Pages
{
    public partial class MyStuffManager : UserControl
    {
        private readonly string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", "modconfig.json");

        public ObservableCollection<Mod> Mods { get; set; }
        private Mod _contextMenuTargetMod;

        public MyStuffManager()
        {
            InitializeComponent();
            LoadMods();
            DataContext = this;
        }

        private void LoadMods()
        {
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                Mods = JsonSerializer.Deserialize<ObservableCollection<Mod>>(json);
            }
            else
            {
                Mods = new ObservableCollection<Mod>();
            }
        }
        
        private void Mod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Mod.IsEnabled))
            {
                SaveMods();
            }
        }

        private void SaveMods()
        {
            var json = JsonSerializer.Serialize(Mods);
            var directory = Path.GetDirectoryName(configFilePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(configFilePath, json);
        }

        private void ImportMod_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Mod files (*.zip;*.brstm;*.szs)|*.zip;*.brstm;*.szs|All files (*.*)|*.*",
                Title = "Select Mod File",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFiles = openFileDialog.FileNames;

                if (selectedFiles.Length == 1)
                {
                    ProcessModFiles(selectedFiles, singleMod: true);
                }
                else
                {
                    var result = MessageBox.Show("Do you want to install each file as its own mod?", "Multiple Files Selected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        ProcessModFiles(selectedFiles, singleMod: false);
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        ProcessModFiles(selectedFiles, singleMod: true);
                    }
                }
            }
        }

        private void ProcessModFiles(string[] filePaths, bool singleMod)
        {
            if (singleMod)
            {
                // Combine files into one mod
                string modName = Path.GetFileNameWithoutExtension(filePaths[0]) + "_combined";
                var modDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", modName);

                if (!Directory.Exists(modDirectory))
                {
                    Directory.CreateDirectory(modDirectory);
                }

                foreach (var file in filePaths)
                {
                    var destFile = Path.Combine(modDirectory, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                }

                if (!Mods.Any(mod => mod.Title.Equals(modName, StringComparison.OrdinalIgnoreCase)))
                {
                    var mod = new Mod
                    {
                        IsEnabled = false,
                        Title = modName,
                        Author = "Unknown"
                    };
                    mod.PropertyChanged += Mod_PropertyChanged;
                    Mods.Add(mod);
                }

                SaveMods();
            }
            else
            {
                // Install each file as its own mod
                foreach (var file in filePaths)
                {
                    var modName = Path.GetFileNameWithoutExtension(file);

                    if (Mods.Any(mod => mod.Title.Equals(modName, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"A mod with the name '{modName}' already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    var modDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", modName);

                    if (!Directory.Exists(modDirectory))
                    {
                        Directory.CreateDirectory(modDirectory);
                    }

                    var destFile = Path.Combine(modDirectory, Path.GetFileName(file));
                    File.Copy(file, destFile, true);

                    var mod = new Mod
                    {
                        IsEnabled = false,
                        Title = modName,
                        Author = "Unknown"
                    };
                    mod.PropertyChanged += Mod_PropertyChanged;
                    Mods.Add(mod);
                }

                SaveMods();
            }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            bool selectAll = Mods.Any(mod => !mod.IsEnabled);
            foreach (var mod in Mods)
            {
                mod.IsEnabled = selectAll;
            }
        }

        private Point startPoint;

        private void ModsListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void ModsListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem == null)
                {
                    return;
                }

                Mod mod = (Mod)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                DataObject dragData = new DataObject("myFormat", mod);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private void ModsListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                Mod mod = e.Data.GetData("myFormat") as Mod;
                ListView listView = sender as ListView;
                Point dropPosition = e.GetPosition(listView);
                int targetIndex = -1;

                for (int i = 0; i < listView.Items.Count; i++)
                {
                    ListViewItem listViewItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(i);
                    if (listViewItem != null)
                    {
                        Rect itemBounds = VisualTreeHelper.GetDescendantBounds(listViewItem);
                        Point itemPosition = listViewItem.TransformToAncestor(listView).Transform(new Point(0, 0));

                        if (dropPosition.Y < itemPosition.Y + itemBounds.Height)
                        {
                            targetIndex = i;
                            break;
                        }
                    }
                }

                if (targetIndex < 0)
                {
                    targetIndex = listView.Items.Count - 1;
                }

                Mods.Remove(mod);
                Mods.Insert(targetIndex, mod);
                SaveMods();
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null && !(current is T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }

        private void ModsListView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListView listView = sender as ListView;
            _contextMenuTargetMod = null;

            if (listView != null)
            {
                ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem != null)
                {
                    _contextMenuTargetMod = (Mod)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                }

                if (_contextMenuTargetMod != null)
                {
                    ContextMenu contextMenu = Resources["ModContextMenu"] as ContextMenu;
                    contextMenu.PlacementTarget = listViewItem;
                    contextMenu.IsOpen = true;
                }
            }
        }

        private void RenameMod_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTargetMod != null)
            {
                string newTitle = Microsoft.VisualBasic.Interaction.InputBox("Enter new title:", "Rename Mod", _contextMenuTargetMod.Title);
                if (!string.IsNullOrEmpty(newTitle) && !Mods.Any(mod => mod.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase)))
                {
                    string oldDirectoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", _contextMenuTargetMod.Title);
                    string newDirectoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", newTitle);

                    Directory.Move(oldDirectoryName, newDirectoryName);
                    _contextMenuTargetMod.Title = newTitle;
                    SaveMods();
                }
                else if (Mods.Any(mod => mod.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"A mod with the name '{newTitle}' already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteMod_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTargetMod != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {_contextMenuTargetMod.Title}?", "Delete Mod", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string modDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", _contextMenuTargetMod.Title);
                    if (Directory.Exists(modDirectory))
                    {
                        Directory.Delete(modDirectory, true);
                    }
                    Mods.Remove(_contextMenuTargetMod);
                    //save that
                    SaveMods();
                }
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTargetMod != null)
            {
                string modDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", _contextMenuTargetMod.Title);
                if (Directory.Exists(modDirectory))
                {
                    System.Diagnostics.Process.Start(modDirectory);
                }
            }
        }
    }
}