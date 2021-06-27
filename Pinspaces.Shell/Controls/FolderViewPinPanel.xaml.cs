using Pinspaces.Core.Controls;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Pinspaces.Shell.Controls
{
    [PinType(DisplayName = "Folder View", PinType = typeof(FolderViewPin))]
    public partial class FolderViewPinPanel : PinUserControl<FolderViewPin>, IDisposable
    {
        private bool disposedValue;
        private FileSystemWatcher fileSystemWatcher;
        private bool pendingRefresh;
        private Window window;

        public FolderViewPinPanel()
        {
            InitializeComponent();
            DataContext = this;

            shellListView.RefreshItems += ShellListView_RefreshItems;

            Loaded += UserControl_Loaded;
            Unloaded += UserControl_Unloaded;
        }

        public ObservableCollection<ShellListItem> Items => shellListView.Items;

        public override void AddContextMenuItems(ContextMenu contextMenu)
        {
            var menuItem = new MenuItem { Header = "Select folder..." };
            menuItem.Click += SelectFolderContextMenuItem_Click;
            contextMenu.Items.Add(menuItem);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DeinitializeFileSystemWatcher();
                }
                disposedValue = true;
            }
        }

        protected override void LoadPin()
        {
            _ = Task.Run(RefreshItems);
            InitializeFileSystemWatcher();
        }

        private void DeinitializeFileSystemWatcher()
        {
            fileSystemWatcher?.Dispose();
            fileSystemWatcher = null;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            pendingRefresh = true;
        }

        private void InitializeFileSystemWatcher()
        {
            DeinitializeFileSystemWatcher();
            fileSystemWatcher = new FileSystemWatcher(Pin.FolderPath);
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private async Task RefreshItems()
        {
            var shellItems = await Task.Run(RetrieveShellItems);
            Items.Clear();
            foreach (var item in shellItems)
            {
                Items.Add(item);
            }
        }

        private Task<IEnumerable<ShellListItem>> RetrieveShellItems()
        {
            var list = new List<ShellListItem>();
            var directoryInfo = new DirectoryInfo(Pin.FolderPath);
            foreach (var info in directoryInfo.EnumerateFileSystemInfos())
            {
                list.Add(new ShellListItem(info));
            }
            return Task.FromResult((IEnumerable<ShellListItem>)list);
        }

        private async void SelectFolderContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Pin.FolderPath = dialog.SelectedPath;
                await RefreshItems();
                NotifyPinPropertyChanged(nameof(Pin.FolderPath));
            }
        }

        private async void ShellListView_RefreshItems(object sender, EventArgs e)
        {
            await RefreshItems();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.Activated += Window_Activated;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            window.Activated -= Window_Activated;
            DeinitializeFileSystemWatcher();
        }

        private async void Window_Activated(object sender, EventArgs e)
        {
            if (pendingRefresh)
            {
                pendingRefresh = false;
                await RefreshItems();
            }
        }
    }
}
