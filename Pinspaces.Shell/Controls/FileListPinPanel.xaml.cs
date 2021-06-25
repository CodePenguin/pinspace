using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pinspaces.Shell.Controls
{
    [PinType(DisplayName = "File List", PinType = typeof(FileListPin))]
    public partial class FileListPinPanel : UserControl, IPinControl
    {
        private FileListPin fileListPin;

        public FileListPinPanel()
        {
            InitializeComponent();
            DataContext = this;

            shellListView.AllowDragReorder = true;
            shellListView.DroppedFiles += ShellListView_DroppedFiles;
            shellListView.RefreshItems += ShellListView_RefreshItems;
            shellListView.KeyUp += ShellListView_KeyUp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Control ContentControl => this;

        public ObservableCollection<ShellListItem> Items => shellListView.Items;

        public void AddContextMenuItems(ContextMenu contextMenu)
        {
            // Do nothing
        }

        public void LoadPin(Guid pinspaceId, Pin pin)
        {
            fileListPin = pin as FileListPin;
            foreach (var file in fileListPin.Files)
            {
                shellListView.AddFile(file);
            }
        }

        private void RefreshItems()
        {
            foreach (var item in shellListView.Items)
            {
                item.Refresh();
            }
        }

        private void ShellListView_DroppedFiles(object sender, DroppedFilesEventArgs e)
        {
            // Add new items to the view
            foreach (var filename in e.Filenames)
            {
                if (shellListView.Items.Any(i => i.Uri == filename))
                {
                    continue;
                }
                shellListView.AddFile(filename, e.TargetItemIndex);
            }

            UpdatePin();
        }

        private void ShellListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                shellListView.RemoveSelectedItems();
                UpdatePin();
                e.Handled = true;
            }
        }

        private void ShellListView_RefreshItems(object sender, EventArgs e)
        {
            RefreshItems();
        }

        private void UpdatePin()
        {
            fileListPin.Files.Clear();
            foreach (var item in shellListView.Items)
            {
                fileListPin.Files.Add(item.Uri);
            }
            PropertyChanged?.Invoke(fileListPin, new PropertyChangedEventArgs(nameof(fileListPin.Files)));
        }
    }
}
