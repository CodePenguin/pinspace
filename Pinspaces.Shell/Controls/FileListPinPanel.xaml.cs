using GongSolutions.Shell;
using GongSolutions.Shell.Interop;
using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pinspaces.Shell.Controls
{
    [PinType(DisplayName = "File List", PinType = typeof(FileListPin))]
    public partial class FileListPinPanel : UserControl, IPinControl, IDropSource
    {
        private FileListPin fileListPin;
        private bool isDragging = false;

        public FileListPinPanel()
        {
            InitializeComponent();
            DataContext = this;

            listView.DragEnter += ListView_DragEnter;
            listView.Drop += ListView_Drop;
            //FIX!!
            //listView.ItemDrag += ListView_ItemDrag;
            listView.MouseRightButtonUp += ListView_MouseRightButtonUp;

            //FIX!!
            //SystemImageList.UseSystemImageList(listView);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Control ContentControl => this;
        public ObservableCollection<FileListItem> Items { get; private set; } = new();

        public void AddContextMenuItems(ContextMenu contextMenu)
        {
            // Do nothing
        }

        public void LoadPin(Pin pin)
        {
            fileListPin = pin as FileListPin;
            foreach (var file in fileListPin.Files)
            {
                AddFile(file, Items.Count);
            }
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

        private void AddFile(string fileName, int index)
        {
            var item = new FileListItem(fileName);
            Items.Insert(index, item);
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
            var targetItem = target?.DataContext as FileListItem;
            var targetItemIndex = Items.IndexOf(targetItem);
            int insertIndex;

            // Move the selected items to the new position
            if (isDragging)
            {
                foreach (FileListItem item in listView.SelectedItems)
                {
                    Items.Remove(item);
                    insertIndex = targetItem != null ? targetItemIndex : Items.Count;
                    Items.Insert(insertIndex, item);
                    targetItem = item;
                    insertIndex = Items.IndexOf(item);
                }
                return;
            }

            // Add new items to the view
            insertIndex = targetItem != null ? targetItemIndex : Items.Count;
            var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var fileName in droppedFiles)
            {
                AddFile(fileName, insertIndex);
            }

            UpdatePin();
        }

        //private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        //{
        //    var selectedItems = SelectedShellItems();
        //    if (selectedItems.Length == 0)
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        isDragging = true;
        //        if (selectedItems.Length == 1)
        //        {
        //            _ = Ole32.DoDragDrop(selectedItems[0].GetIDataObject(), this, DragDropEffects.All, out _);
        //            return;
        //        }

        //        // Convert selected files to a DataObject of file names
        //        var dataObject = new DataObject();
        //        var fileNames = new StringCollection();
        //        foreach (var file in selectedItems)
        //        {
        //            fileNames.Add(file.FileSystemPath);
        //        }
        //        dataObject.SetFileDropList(fileNames);
        //        _ = Ole32.DoDragDrop(dataObject, this, DragDropEffects.All, out _);
        //    }
        //    finally
        //    {
        //        isDragging = false;
        //    }
        //}

        private void ListView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedItems = SelectedShellItems();
            if (selectedItems.Length == 0)
            {
                return;
            }
            var contextMenu = new ShellContextMenu(selectedItems);
            var mousePos = e.GetPosition(listView);
            //FIX!!
            //contextMenu.ShowContextMenu(listView, mousePos));
        }

        private ShellItem[] SelectedShellItems()
        {
            var items = new List<ShellItem>();
            foreach (var item in listView.SelectedItems)
            {
                var fileListItem = item as FileListItem;
                items.Add(new ShellItem(fileListItem.Uri));
            }
            return items.ToArray();
        }

        private void UpdatePin()
        {
            fileListPin.Files.Clear();
            foreach (var item in Items)
            {
                fileListPin.Files.Add(item.Uri);
            }
            PropertyChanged?.Invoke(fileListPin, new PropertyChangedEventArgs(nameof(fileListPin.Files)));
        }
    }
}
