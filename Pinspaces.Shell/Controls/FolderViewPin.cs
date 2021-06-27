using Pinspaces.Core.Data;
using System;
using System.IO;

namespace Pinspaces.Shell.Controls
{
    public class FolderViewPin : Pin
    {
        private string folderPath = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

        public string FolderPath { get => folderPath; set => SetProperty(ref folderPath, value); }
    }
}
