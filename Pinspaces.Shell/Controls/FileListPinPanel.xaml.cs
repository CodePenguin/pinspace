using Pinspaces.Core.Controls;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Pinspaces.Shell.Controls
{
    [PinType(DisplayName = "File List", PinType = typeof(FileListPin))]
    public partial class FileListPinPanel : PinUserControl<FileListPin>
    {
        public FileListPinPanel()
        {
            InitializeComponent();
            DataContext = this;

            shellListView.AllowDragReorder = true;
            shellListView.DroppedFiles += ShellListView_DroppedFiles;
            shellListView.RefreshItems += ShellListView_RefreshItems;
            shellListView.KeyUp += ShellListView_KeyUp;
        }

        public ObservableCollection<ShellListItem> Items => shellListView.Items;

        protected override void LoadPin()
        {
            foreach (var file in Pin.Files)
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
            Pin.Files.Clear();
            foreach (var item in shellListView.Items)
            {
                Pin.Files.Add(item.Uri);
            }
            Pin.NotifyFilesChanged();
        }
    }
}
