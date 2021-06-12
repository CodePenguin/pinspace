using GongSolutions.Shell;
using GongSolutions.Shell.Interop;
using Pinspaces.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pinspaces.Shell.Controls
{
    public class DroppedFilesEventArgs : EventArgs
    {
        public string[] Filenames { get; set; }
        public bool FromSelf { get; set; }
        public ShellListItem TargetItem { get; set; }
        public int TargetItemIndex { get; set; }
    }

    public partial class ShellListView : ListView, IDropSource
    {
        protected bool isDragging = false;
        private readonly List<ShellListItem> selectedItems = new();
        private Point? startingOffset = null;

        public ShellListView()
        {
            InitializeComponent();
            DataContext = this;

            DragEnter += ListView_DragEnter;
            Drop += ListView_Drop;
            MouseDoubleClick += ListView_MouseDoubleClick;
            MouseMove += ListView_MouseMove;
            MouseUp += ListView_MouseUpEvent;
            PreviewMouseDown += ListView_PreviewMouseDown;
        }

        public event EventHandler<DroppedFilesEventArgs> DroppedFiles;

        public event EventHandler<EventArgs> RefreshItems;

        public bool AllowDragReorder { get; set; } = false;

        public ObservableCollection<ShellListItem> Items { get; private set; } = new();

        public static void OpenItem(ShellListItem item)
        {
            if (!item.Error)
            {
                var process = new ProcessStartInfo
                {
                    FileName = item.ShellItem.FileSystemPath,
                    UseShellExecute = true
                };
                Process.Start(process);
            }
        }

        public void AddFile(string filename, int index)
        {
            var item = new ShellListItem(filename);
            Items.Insert(index, item);
        }

        public void AddFile(string fileName)
        {
            AddFile(fileName, Items.Count);
        }

        public void AddItem(ShellListItem item)
        {
            Items.Add(item);
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
                RefreshItems?.Invoke(this, new EventArgs());
                e.Handled = true;
            }
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            var target = ((DependencyObject)e.OriginalSource).FindParent<ListViewItem>();
            var targetItem = target?.DataContext as ShellListItem;
            var targetItemIndex = targetItem != null ? Items.IndexOf(targetItem) : Items.Count;
            var droppedFilenames = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Move the selected items to the new position
            if (isDragging && AllowDragReorder)
            {
                foreach (var item in selectedItems)
                {
                    Items.Remove(item);
                    var insertIndex = targetItem != null ? targetItemIndex : Items.Count;
                    Items.Insert(insertIndex, item);
                    targetItem = item;
                    targetItemIndex = Items.IndexOf(item);
                }
                selectedItems.Clear();
                return;
            }

            // Handle dropped files
            DroppedFiles?.Invoke(this, new DroppedFilesEventArgs
            {
                Filenames = droppedFilenames,
                TargetItem = targetItem,
                TargetItemIndex = targetItemIndex
            });
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext;
            if (item is ShellListItem shellListItem)
            {
                OpenItem(shellListItem);
            }
        }

        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (startingOffset == null)
            {
                return;
            }
            var offset = e.GetPosition(this) - startingOffset.Value;
            if ((e.RightButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed) && (Math.Abs(offset.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(offset.Y) > SystemParameters.MinimumVerticalDragDistance))
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
                var mousePos = PointToScreen(e.GetPosition(this));
                contextMenu.ShowContextMenu(new System.Drawing.Point(Convert.ToInt32(mousePos.X), Convert.ToInt32(mousePos.Y)));
            }
        }

        private void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            startingOffset = e.GetPosition(this);
            selectedItems.Clear();
            selectedItems.AddRange(SelectedItems.Cast<ShellListItem>());
            if (selectedItems.Count == 0)
            {
                var mouseItem = VisualTreeHelper.HitTest(this, e.GetPosition(this)).VisualHit.FindParent<ListViewItem>();
                if (mouseItem != null)
                {
                    selectedItems.Add(mouseItem.DataContext as ShellListItem);
                }
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
