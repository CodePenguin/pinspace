using GongSolutions.Shell;
using GongSolutions.Shell.Interop;
using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

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

            //listView.DragEnter += ListView_DragEnter;
            //listView.DragDrop += ListView_DragDrop;
            //listView.ItemDrag += ListView_ItemDrag;
            //listView.MouseClick += ListView_MouseClick;
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

        //FIX!!
        //private void ListView_DragDrop(object sender, DragEventArgs e)
        //{
        //    listView.BeginUpdate();
        //    try
        //    {
        //        var dropPoint = listView.PointToClient(new Point(e.X, e.Y));
        //        var targetItem = listView.GetItemAt(dropPoint.X, dropPoint.Y);
        //        int insertIndex;

        //        // Move the selected items to the new position
        //        if (isDragging)
        //        {
        //            foreach (ListViewItem item in listView.SelectedItems)
        //            {
        //                item.Remove();
        //                insertIndex = targetItem != null ? targetItem.Index : listView.Items.Count;
        //                listView.Items.Insert(insertIndex, item);
        //                targetItem = item;
        //                insertIndex = item.Index;
        //            }
        //            return;
        //        }

        //        // Add new items to the view
        //        insertIndex = targetItem != null ? targetItem.Index : listView.Items.Count;
        //        var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
        //        foreach (var fileName in droppedFiles)
        //        {
        //            AddFile(fileName, insertIndex);
        //        }

        //        UpdatePin();
        //    }
        //    finally
        //    {
        //        listView.EndUpdate();
        //    }
        //}

        //private void ListView_DragEnter(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        e.Effect = DragDropEffects.Copy;
        //    }
        //}

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

        //private void ListView_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button != MouseButtons.Right)
        //    {
        //        return;
        //    }
        //    var selectedItems = SelectedShellItems();
        //    if (selectedItems.Length == 0)
        //    {
        //        return;
        //    }
        //    var contextMenu = new ShellContextMenu(selectedItems);
        //    contextMenu.ShowContextMenu(listView, e.Location);
        //}

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
