using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using Pinspaces.Shell.Git;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Pinspaces.Shell.Controls
{
    [PinType(DisplayName = "Git Folder View", PinType = typeof(GitFolderViewPin))]
    public partial class GitFolderViewPinPanel : UserControl, IPinControl
    {
        private FileSystemWatcher fileSystemWatcher;
        private bool pendingRefresh;
        private GitFolderViewPin pin;
        private Window window;

        public GitFolderViewPinPanel()
        {
            InitializeComponent();
            DataContext = this;

            shellListView.RefreshItems += ShellListView_RefreshItems;

            Loaded += UserControl_Loaded;
            Unloaded += UserControl_Unloaded;
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
            this.pin = pin as GitFolderViewPin;
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
            fileSystemWatcher = new FileSystemWatcher(pin.RepositoryPath);
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private async Task RefreshItems()
        {
            var gitStatusItems = await Task.Run(RetrieveGitStatusItems);
            shellListView.Items.Clear();
            foreach (var entry in gitStatusItems)
            {
                shellListView.AddItem(entry);
            }
            pendingRefresh = false;
        }

        private async Task<IEnumerable<GitShellListItem>> RetrieveGitStatusItems()
        {
            var list = new List<GitShellListItem>();
            var git = new GitInterop(pin.RepositoryPath);
            var entries = await git.StatusAsync();
            foreach (var entry in entries)
            {
                var filePath = Path.Join(pin.RepositoryPath, entry.ToPath.Replace('/', '\\'));

                var item = new GitShellListItem(filePath, entry);
                list.Add(item);
            }
            return list;
        }

        private async void SelectFolderContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var git = new GitInterop(dialog.SelectedPath);
                var isGitRepository = await git.IsGitRepositoryAsync();
                if (!isGitRepository)
                {
                    _ = MessageBox.Show("The selected folder is not a Git repository.", "Pinspaces", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                pin.RepositoryPath = dialog.SelectedPath;
                await RefreshItems();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pin.RepositoryPath)));
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
