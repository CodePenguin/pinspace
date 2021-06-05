using GongSolutions.Shell;
using System;
using System.IO;

namespace Pinspaces.Shell.Controls
{
    public class FileListItem
    {
        private ShellItem shellItem;

        public FileListItem(string uri)
        {
            Uri = uri;
            Refresh();
        }

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
                shellItem = new ShellItem(Uri);
                var fileInfo = new FileInfo(shellItem.FileSystemPath);
                DisplayName = shellItem.DisplayName;
                FileTypeDescription = shellItem.FileTypeDescription;
                LastModifiedDateTime = fileInfo.LastWriteTime;

                if (!shellItem.IsFolder || fileInfo.Extension.Equals(".zip", StringComparison.CurrentCultureIgnoreCase))
                {
                    Size = (int)Math.Ceiling(fileInfo.Length / 1024d);
                }
                else
                {
                    Size = 0;
                }
                Error = false;
            }
            catch (Exception)
            {
                shellItem = null;
                DisplayName = Path.GetFileName(Uri);
                FileTypeDescription = string.Empty;
                LastModifiedDateTime = DateTime.MinValue;
                Size = 0;
                Error = true;
            }
        }
    }
}
