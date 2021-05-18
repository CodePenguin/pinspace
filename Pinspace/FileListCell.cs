using GongSolutions.Shell;
using GongSolutions.Shell.Interop;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Pinspace
{
    [DisplayName("File List")]
    public class FileListCell : PinboardCell, IDropSource
    {
        private readonly List<ShellItem> files = new List<ShellItem>();
        private bool isDragging = false;
        private ListView listView;

        public FileListCell() : base()
        {
            // FIX!!! REMOVE ME!!!
            // FIX!!! REMOVE ME!!!
            // FIX!!! REMOVE ME!!!
            // FIX!!! REMOVE ME!!!
            // FIX!!! REMOVE ME!!!
            AddFile("d:\\temp\\temp.txt", listView.Items.Count);
            AddFile("d:\\temp\\temp_before.txt", listView.Items.Count);
            AddFile("d:\\temp\\temp_after.txt", listView.Items.Count);
            // FIX!!! REMOVE ME!!!
            // FIX!!! REMOVE ME!!!
            // FIX!!! REMOVE ME!!!
            // FIX!!! REMOVE ME!!!
            // FIX!!! REMOVE ME!!!
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

        protected override void InitializeControl()
        {
            listView = new ListView
            {
                AllowDrop = true,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                MultiSelect = true,
                View = View.Details,
            };
            Controls.Add(listView);

            listView.DragEnter += ListView_DragEnter;
            listView.DragDrop += ListView_DragDrop;
            listView.ItemDrag += ListView_ItemDrag;
            listView.MouseClick += ListView_MouseClick;
            SystemImageList.UseSystemImageList(listView);

            listView.Columns.Add("Name", 300);
            listView.Columns.Add("Date modified", 140);
            listView.Columns.Add("Type", 200);
            listView.Columns.Add("Size", 100);

            base.InitializeControl();
        }

        private void AddFile(string fileName, int index)
        {
            var shellItem = new ShellItem(fileName);
            files.Add(shellItem);

            var item = new ListViewItem { Tag = files.Count - 1 };
            UpdateListViewItem(item);
            listView.Items.Insert(index, item);
        }

        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            listView.BeginUpdate();
            try
            {
                var dropPoint = listView.PointToClient(new Point(e.X, e.Y));
                var targetItem = listView.GetItemAt(dropPoint.X, dropPoint.Y);
                int insertIndex;

                // Move the selected items to the new position
                if (isDragging)
                {
                    foreach (ListViewItem item in listView.SelectedItems)
                    {
                        item.Remove();
                        insertIndex = targetItem != null ? targetItem.Index : listView.Items.Count;
                        listView.Items.Insert(insertIndex, item);
                        targetItem = item;
                        insertIndex = item.Index;
                    }
                    return;
                }

                // Add new items to the view
                insertIndex = targetItem != null ? targetItem.Index : listView.Items.Count;
                var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var fileName in droppedFiles)
                {
                    AddFile(fileName, insertIndex);
                }
            }
            finally
            {
                listView.EndUpdate();
            }
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var selectedItems = SelectedShellItems();
            if (selectedItems.Length == 0)
            {
                return;
            }
            try
            {
                isDragging = true;
                if (selectedItems.Length == 1)
                {
                    Ole32.DoDragDrop(selectedItems[0].GetIDataObject(), this, DragDropEffects.All, out _);
                    return;
                }

                // Convert selected files to a DataObject of file names
                var dataObject = new DataObject();
                var fileNames = new StringCollection();
                foreach (var file in selectedItems)
                {
                    fileNames.Add(file.FileSystemPath);
                }
                dataObject.SetFileDropList(fileNames);
                Ole32.DoDragDrop(dataObject, this, DragDropEffects.All, out _);
            }
            finally
            {
                isDragging = false;
            }
        }

        private void ListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            var selectedItems = SelectedShellItems();
            if (selectedItems.Length == 0)
            {
                return;
            }
            var contextMenu = new ShellContextMenu(selectedItems);
            contextMenu.ShowContextMenu(listView, e.Location);
        }

        private ShellItem[] SelectedShellItems()
        {
            var items = new List<ShellItem>();
            foreach (ListViewItem item in listView.SelectedItems)
            {
                items.Add(files[(int)item.Tag]);
            }
            return items.ToArray();
        }

        private void UpdateListViewItem(ListViewItem item)
        {
            var shellItem = files[(int)item.Tag];
            var fileInfo = new FileInfo(shellItem.FileSystemPath);
            item.ImageIndex = shellItem.GetSystemImageListIndex(ShellIconType.SmallIcon, ShellIconFlags.OverlayIndex);
            item.SubItems.Clear();
            item.Text = shellItem.DisplayName;
            item.SubItems.Add(fileInfo.LastWriteTime.ToString());
            item.SubItems.Add(shellItem.FileTypeDescription);

            if (!shellItem.IsFolder || fileInfo.Extension.Equals(".zip", System.StringComparison.CurrentCultureIgnoreCase))
            {
                var size = System.Math.Ceiling(fileInfo.Length / 1024d);
                item.SubItems.Add($"{size:n0} KB");
            }
        }
    }
}
