using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using Pinspaces.Shell.Git;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Pinspaces.Shell.Controls
{
    [PinType(DisplayName = "Git Folder View", PinType = typeof(GitFolderViewPin))]
    public partial class GitFolderViewPinPanel : UserControl, IPinControl
    {
        private GitFolderViewPin pin;

        public GitFolderViewPinPanel()
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
            this.pin = pin as GitFolderViewPin;
            RefreshItems();
        }

        private void RefreshItems()
        {
            Items.Clear();
            var git = new Git.GitInterop(pin.RepositoryPath);
            var entries = git.Status();
            foreach (var entry in entries)
            {
                var filePath = Path.Join(pin.RepositoryPath, entry.ToPath.Replace('/', '\\'));

                var item = new GitShellListItem(filePath, entry);
                shellListView.AddItem(item);
            }
        }

        private void SelectFolderContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var git = new GitInterop(dialog.SelectedPath);
                    if (!git.IsGitRepository)
                    {
                        MessageBox.Show("The selected folder is not a Git repository.", "Pinspaces", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    pin.RepositoryPath = dialog.SelectedPath;
                    RefreshItems();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pin.RepositoryPath)));
                }
            }
        }

        private void ShellListView_RefreshItems(object sender, EventArgs e)
        {
            RefreshItems();
        }
    }
}
