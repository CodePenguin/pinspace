using GongSolutions.Shell;
using System;
using System.ComponentModel;
using System.IO;

namespace Pinspaces.Shell.Controls
{
    public class ShellListItem : INotifyPropertyChanged
    {
        private ShellItem shellItem;

        public ShellListItem(string uri)
        {
            Uri = uri;
            Refresh();
        }

        public ShellListItem(FileSystemInfo info)
        {
            Uri = info.FullName;
            shellItem = new ShellItem(Uri);
            Refresh(info);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName { get; private set; }
        public bool Error { get; private set; }
        public string FileTypeDescription { get; private set; }
        public DateTime LastModifiedDateTime { get; private set; }
        public int Size { get; private set; }
        public string Uri { get; private set; }

        public IntPtr Pidl => shellItem != null ? shellItem.Pidl : IntPtr.Zero;

        public void Refresh()
        {
            try
            {
                if (shellItem == null)
                {
                    shellItem = new ShellItem(Uri);
                }
                if (shellItem.IsFolder)
                {
                    Refresh(new DirectoryInfo(shellItem.FileSystemPath));
                }
                else
                {
                    Refresh(new FileInfo(shellItem.FileSystemPath));
                }
            }
            catch (Exception)
            {
                shellItem = null;
                DisplayName = Path.GetFileName(Uri);
                FileTypeDescription = string.Empty;
                LastModifiedDateTime = DateTime.MinValue;
                Size = 0;
                Error = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        private void Refresh(FileSystemInfo fileSystemInfo)
        {
            DisplayName = shellItem.DisplayName;
            FileTypeDescription = shellItem.FileTypeDescription;
            LastModifiedDateTime = fileSystemInfo.LastWriteTime;

            if (fileSystemInfo is DirectoryInfo)
            {
                Size = 0;
            }
            else if (fileSystemInfo is FileInfo fileInfo)
            {
                Size = (int)Math.Ceiling(fileInfo.Length / 1024d);
            }
            Error = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
