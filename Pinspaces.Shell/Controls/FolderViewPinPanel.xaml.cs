using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Pinspaces.Shell.Controls
{
    [PinType(DisplayName = "Folder View", PinType = typeof(FolderViewPin))]
    public partial class FolderViewPinPanel : UserControl, IPinControl
    {
        private FolderViewPin pin;

        public FolderViewPinPanel()
        {
            InitializeComponent();
            DataContext = this;

            shellListView.RefreshItems += ShellListView_RefreshItems;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Control ContentControl => this;

        public ObservableCollection<ShellListItem> Items => shellListView.Items;

        public void AddContextMenuItems(ContextMenu contextMenu)
        {
            var menuItem = new MenuItem { Header = "Select folder..." };
            menuItem.Click += SelectFolderContextMenuItem_Click;
            contextMenu.Items.Add(menuItem);
        }

        public void LoadPin(Guid pinspaceId, Pin pin)
        {
            this.pin = pin as FolderViewPin;
            RefreshItems();
        }

        private void RefreshItems()
        {
            Items.Clear();
            var directoryInfo = new DirectoryInfo(pin.FolderPath);
            foreach (var info in directoryInfo.EnumerateFileSystemInfos())
            {
                Items.Add(new ShellListItem(info));
            }
        }

        private void SelectFolderContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pin.FolderPath = dialog.SelectedPath;
                RefreshItems();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pin.FolderPath)));
            }
        }

        private void ShellListView_RefreshItems(object sender, EventArgs e)
        {
            RefreshItems();
        }
    }
}
