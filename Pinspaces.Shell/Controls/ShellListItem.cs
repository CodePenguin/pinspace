using GongSolutions.Shell;
using System;
using System.ComponentModel;
using System.IO;

namespace Pinspaces.Shell.Controls
{
    public class ShellListItem : INotifyPropertyChanged
    {
        public ShellListItem(string uri)
        {
            Uri = uri;
            Refresh();
        }

        public ShellListItem(FileSystemInfo info)
        {
            Uri = info.FullName;
            ShellItem = new ShellItem(Uri);
            Refresh(info);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName { get; private set; }
        public bool Error { get; private set; }
        public string FileTypeDescription { get; private set; }
        public DateTime LastModifiedDateTime { get; private set; }
        public ShellItem ShellItem { get; private set; }
        public int Size { get; private set; }
        public string Uri { get; private set; }
        public IntPtr Pidl => ShellItem != null ? ShellItem.Pidl : IntPtr.Zero;

        public void Refresh()
        {
            try
            {
                if (ShellItem == null)
                {
                    ShellItem = new ShellItem(Uri);
                }
                if (ShellItem.IsFolder)
                {
                    Refresh(new DirectoryInfo(ShellItem.FileSystemPath));
                }
                else
                {
                    Refresh(new FileInfo(ShellItem.FileSystemPath));
                }
            }
            catch (Exception)
            {
                ShellItem = null;
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
            DisplayName = ShellItem.DisplayName;
            FileTypeDescription = ShellItem.FileTypeDescription;
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
