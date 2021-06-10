using GongSolutions.Shell;
using GongSolutions.Shell.Interop;
using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pinspaces.Shell.Controls
{
    [PinType(DisplayName = "Folder View", PinType = typeof(FolderViewPin))]
    public partial class FolderViewPinPanel : UserControl, IPinControl, IDropSource
    {
        private readonly List<ShellListItem> selectedItems = new();
        private bool isDragging = false;
        private FolderViewPin pin;
        private Point? startingOffset = null;

        public FolderViewPinPanel()
        {
            InitializeComponent();
            DataContext = this;

            listView.PreviewMouseDown += ListView_PreviewMouseDown;
            listView.MouseMove += ListView_MouseMove;
            listView.MouseUp += ListView_MouseUpEvent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Control ContentControl => this;

        public ObservableCollection<ShellListItem> Items { get; private set; } = new();

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

        HResult IDropSource.GiveFeedback(int dwEffect)
        {
            return HResult.DRAGDROP_S_USEDEFAULTCURSORS;
        }

        HResult IDropSource.QueryContinueDrag(bool fEscapePressed, int grfKeyState)
        {
            if (fEscapePressed)
            {
                return HResult.DRAGDROP_S_CANCEL;
            }
            else if ((grfKeyState & (int)(MK.MK_LBUTTON | MK.MK_RBUTTON)) == 0)
            {
                return HResult.DRAGDROP_S_DROP;
            }
            else
            {
                return HResult.S_OK;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.F5)
            {
                RefreshItems();
                e.Handled = true;
            }
        }

        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (startingOffset == null)
            {
                return;
            }
            var offset = e.GetPosition(listView) - startingOffset.Value;
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(offset.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(offset.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                StartItemDragOperation();
            }
        }

        private void ListView_MouseUpEvent(object sender, MouseButtonEventArgs e)
        {
            startingOffset = null;
            if (e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Released)
            {
                var selectedItems = SelectedShellItems();
                if (selectedItems.Length == 0)
                {
                    return;
                }
                var contextMenu = new ShellContextMenu(selectedItems);
                var mousePos = PointToScreen(e.GetPosition(listView));
                contextMenu.ShowContextMenu(new System.Drawing.Point(Convert.ToInt32(mousePos.X), Convert.ToInt32(mousePos.Y)));
            }
        }

        private void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            startingOffset = e.GetPosition(listView);
            selectedItems.Clear();
            selectedItems.AddRange(listView.SelectedItems.Cast<ShellListItem>());
            if (selectedItems.Count == 0)
            {
                var mouseItem = VisualTreeHelper.HitTest(listView, e.GetPosition(listView)).VisualHit.FindParent<ListViewItem>();
                if (mouseItem != null)
                {
                    selectedItems.Add(mouseItem.DataContext as ShellListItem);
                }
            }
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

        private ShellItem[] SelectedShellItems()
        {
            var items = new List<ShellItem>();
            foreach (var item in selectedItems)
            {
                if (item.Error)
                {
                    continue;
                }
                items.Add(new ShellItem(item.Uri));
            }
            return items.ToArray();
        }

        private void SelectFolderContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pin.FolderPath = dialog.SelectedPath;
                    RefreshItems();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pin.FolderPath)));
                }
            }
        }

        private void StartItemDragOperation()
        {
            var shellItems = SelectedShellItems();
            if (shellItems.Length == 0)
            {
                return;
            }
            try
            {
                isDragging = true;
                if (shellItems.Length == 1)
                {
                    _ = Ole32.DoDragDrop(shellItems[0].GetIDataObject(), this, System.Windows.Forms.DragDropEffects.All, out _);
                    return;
                }

                // Convert selected files to a DataObject of file names
                var dataObject = new DataObject();
                var fileNames = new StringCollection();
                foreach (var file in shellItems)
                {
                    fileNames.Add(file.FileSystemPath);
                }
                dataObject.SetFileDropList(fileNames);
                _ = Ole32.DoDragDrop(dataObject, this, System.Windows.Forms.DragDropEffects.All, out _);
            }
            finally
            {
                isDragging = false;
            }
        }
    }
}
