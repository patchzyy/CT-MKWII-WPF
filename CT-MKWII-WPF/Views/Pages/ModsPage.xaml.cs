﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Threading;
using CT_MKWII_WPF.Utils;


namespace CT_MKWII_WPF.Views.Pages
{
    public partial class ModsPage : Page
    {
        //dont do this, we do it later
        private readonly string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", "modconfig.json");
        public ObservableCollection<Mod> Mods { get; set; }
        private Mod _contextMenuTargetMod;
        
        //public ObservableCollection<ModItem> Mods { get; set; }
        private Point startPoint;
        public ModsPage()
        {
            InitializeComponent();
            LoadMods();
            ModsListView.DataContext = this;
        }
        
        private void LoadMods()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    var json = File.ReadAllText(configFilePath);
                    Mods = JsonSerializer.Deserialize<ObservableCollection<Mod>>(json) ?? new ObservableCollection<Mod>();
                }
                else
                {
                    Mods = new ObservableCollection<Mod>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load mods: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mods = new ObservableCollection<Mod>();
            }

            foreach (var mod in Mods)
            {
                mod.PropertyChanged += Mod_PropertyChanged;
            }
            UpdateEmptyListMessageVisibility();
        }
        private void UpdateEmptyListMessageVisibility()
        {
            EmptyListMessage.Visibility = Mods.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ModsListView.Visibility = Mods.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        
        private void Mod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Mod.IsEnabled))
            {
                SaveMods();
            }
            UpdateEmptyListMessageVisibility();
        }
        
        private void ModsListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }
        
        private void ModsListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem != null)
                {
                    Mod mod = (Mod)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                    DataObject dragData = new DataObject("myFormat", mod);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
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
        
        private void ModsListView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("myFormat"))
                {
                    Mod mod = e.Data.GetData("myFormat") as Mod;
                    ListView listView = sender as ListView;
                    Point dropPosition = e.GetPosition(listView);
                    int targetIndex = GetDropTargetIndex(listView, dropPosition);

                    if (targetIndex < 0)
                    {

                        targetIndex = listView.Items.Count - 1;
                    }

                    MoveModInCollection(mod, targetIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to drop mod: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void MoveModInCollection(Mod mod, int targetIndex)
        {
            Mods.Remove(mod);
            Mods.Insert(targetIndex, mod);
            SaveMods();
        }
        
        private void SaveMods()
        {
            try
            {
                var json = JsonSerializer.Serialize(Mods);
                var directory = Path.GetDirectoryName(configFilePath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(configFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save mods: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private int GetDropTargetIndex(ListView listView, Point dropPosition)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                ListViewItem listViewItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(i);
                if (listViewItem != null)
                {
                    Rect itemBounds = VisualTreeHelper.GetDescendantBounds(listViewItem);
                    Point itemPosition = listViewItem.TransformToAncestor(listView).Transform(new Point(0, 0));

                    if (dropPosition.Y < itemPosition.Y + itemBounds.Height)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
        
        private void ModsListView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem != null)
                {
                    _contextMenuTargetMod = (Mod)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                    if (_contextMenuTargetMod != null)
                    {
                        ContextMenu contextMenu = Resources["ModContextMenu"] as ContextMenu;
                        if (contextMenu != null)
                        {
                            contextMenu.PlacementTarget = listViewItem;
                            contextMenu.IsOpen = true;
                        }
                    }
                }
            }
        }

        private void ImportmodClick(object sender, RoutedEventArgs e)
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
                    var result = MessageBox.Show("Do you want to combine all files into 1 mod?", "Multiple Files Selected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        ProcessModFiles(selectedFiles, singleMod: false);
                    }
                    else if (result == MessageBoxResult.Yes)
                    {
                        ProcessModFiles(selectedFiles, singleMod: true);
                    }
                }
            }
        }
        
        private async void ProcessModFiles(string[] filePaths, bool singleMod)
        {
            ShowLoading(true);
            try
            {
                if (singleMod)
                {
                    await CombineFilesIntoSingleMod(filePaths);
                }
                else
                {
                    await InstallEachFileAsMod(filePaths);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to process mod files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ShowLoading(false);
        }
        
        private bool ModExists(string modName)
        {
            return Mods.Any(mod => mod.Title.Equals(modName, StringComparison.OrdinalIgnoreCase));
        }
        
        private async Task InstallEachFileAsMod(string[] filePaths)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                UpdateProgress(i + 1, filePaths.Length);
                var modName = Path.GetFileNameWithoutExtension(filePaths[i]);

                if (ModExists(modName))
                {
                    MessageBox.Show($"A mod with the name '{modName}' already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }

                var modDirectory = GetModDirectoryPath(modName);
                CreateDirectory(modDirectory);
                await Task.Run(() => ProcessFile(filePaths[i], modDirectory));
                AddMod(modName);
            }
        }
        
        private async Task CombineFilesIntoSingleMod(string[] filePaths)
        {
            string modName = Microsoft.VisualBasic.Interaction.InputBox("Enter mod name:", "Mod Name", "New Mod");
            var modDirectory = GetModDirectoryPath(modName);
            CreateDirectory(modDirectory);
            for (int i = 0; i < filePaths.Length; i++)
            {
                UpdateProgress(i + 1, filePaths.Length);
                await Task.Run(() => ProcessFile(filePaths[i], modDirectory));
            }
            AddMod(modName);
        }
        
        private void AddMod(string modName)
        {
            if (!Mods.Any(mod => mod.Title.Equals(modName, StringComparison.OrdinalIgnoreCase)))
            {
                var mod = new Mod
                {
                    IsEnabled = false,
                    Title = modName,
                };
                mod.PropertyChanged += Mod_PropertyChanged;
                Mods.Add(mod);
                SaveMods();
            }
            UpdateEmptyListMessageVisibility();
        }
        
        private void ProcessFile(string file, string destinationDirectory)
        {
            if (Path.GetExtension(file).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                // Ensure the destination directory exists
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                try
                {
                    // Extract the zip file to the destination directory
                    //get name of the zip file
                    string zipFileName = Path.GetFileNameWithoutExtension(file);
                    //now we check if there isnt already a folder with the same name as the zip file, if so... cancel
                    if (Directory.Exists(Path.Combine(destinationDirectory, zipFileName)))
                    {
                        MessageBox.Show($"You already have a mod with this name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ZipFile.ExtractToDirectory(file, destinationDirectory);
                }
                //if file already exists, we catch the exception and show a message
                catch (IOException)
                {
                    MessageBox.Show($"You already have a mod with this name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to extract zip file.\nThis is most likely because there is an invalid folder name. Or the ZIP might be password protected\n {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    // Ensure the destination directory exists
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    // Copy the file to the destination directory
                    var destFile = Path.Combine(destinationDirectory, Path.GetFileName(file));
                    File.Copy(file, destFile, overwrite: true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to copy file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void UpdateProgress(int current, int total)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = (double)current / total * 100;
                StatusTextBlock.Text = $"Processing {current} of {total} files...";
            }, DispatcherPriority.Background);
        }
        
        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        private static string GetModDirectoryPath(string modName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", modName);
        }
        
        private void ShowLoading(bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
                StatusTextBlock.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void EnableClick(object sender, RoutedEventArgs e)
        {
            bool selectAll = Mods.Any(mod => !mod.IsEnabled);
            foreach (var mod in Mods)
            {
                mod.IsEnabled = selectAll;
            }
        }
        
        private void RenameMod_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTargetMod != null)
            {
                string newTitle = Microsoft.VisualBasic.Interaction.InputBox("Enter new title:", "Rename Mod", _contextMenuTargetMod.Title);
                if (!string.IsNullOrWhiteSpace(newTitle) && !Mods.Any(mod => mod.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase)))
                {
                    RenameModDirectory(newTitle);
                    _contextMenuTargetMod.Title = newTitle;
                    SaveMods();
                }
                else if (Mods.Any(mod => mod.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"A mod with the name '{newTitle}' already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void RenameModDirectory(string newTitle)
        {
            try
            {
                string oldDirectoryName = GetModDirectoryPath(_contextMenuTargetMod.Title);
                string newDirectoryName = GetModDirectoryPath(newTitle);

                Directory.Move(oldDirectoryName, newDirectoryName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to rename mod directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteMod_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTargetMod != null && MessageBox.Show($"Are you sure you want to delete {_contextMenuTargetMod.Title}?", "Delete Mod", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DeleteModDirectory();
                Mods.Remove(_contextMenuTargetMod);
                SaveMods();
            }
            UpdateEmptyListMessageVisibility();
        }

        private void DeleteModDirectory()
        {
            try
            {
                string modDirectory = GetModDirectoryPath(_contextMenuTargetMod.Title);
                if (Directory.Exists(modDirectory))
                {
                    Directory.Delete(modDirectory, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete mod directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_contextMenuTargetMod != null)
                {
                    string modDirectory = GetModDirectoryPath(_contextMenuTargetMod.Title);
                    if (Directory.Exists(modDirectory))
                    {
                        System.Diagnostics.Process.Start("explorer", modDirectory);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open mod folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
}