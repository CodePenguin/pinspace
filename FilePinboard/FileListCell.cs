using GongSolutions.Shell;
using GongSolutions.Shell.Interop;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FilePinboard
{
    public class FileListCell : PinboardCell, IDropSource
    {
        private readonly ListView listView;
        private readonly List<ShellItem> files = new List<ShellItem>();

        public FileListCell() : base()
        {
            listView = new ListView
            {
                AllowDrop = true,
                Dock = DockStyle.Fill,
                MultiSelect = true,
                FullRowSelect = true,
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
        }

        private void AddFile(string fileName)
        {
            var shellItem = new ShellItem(fileName);
            files.Add(shellItem);

            var item = new ListViewItem { Tag = files.Count - 1 };
            UpdateListViewItem(item);
            listView.Items.Add(item);
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            listView.BeginUpdate();
            try
            {
                var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var fileName in droppedFiles)
                {
                    AddFile(fileName);
                }
            } 
            finally
            {
                listView.EndUpdate();
            }
        }

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var selectedItems = SelectedItems();
            if (selectedItems.Length == 0)
            {
                return;
            }
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

        private void ListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            var selectedItems = SelectedItems();
            if (selectedItems.Length == 0)
            {
                return;
            }
            var contextMenu = new ShellContextMenu(selectedItems);
            contextMenu.ShowContextMenu(listView, e.Location);
        }

        private ShellItem[] SelectedItems()
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

        HResult IDropSource.GiveFeedback(int dwEffect)
        {
            return HResult.DRAGDROP_S_USEDEFAULTCURSORS;
        }
    }
}
