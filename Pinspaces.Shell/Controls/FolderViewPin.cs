using Pinspaces.Core.Data;
using System;
using System.IO;

namespace Pinspaces.Shell.Controls
{
    public class FolderViewPin : Pin
    {
        public FolderViewPin()
        {
            FolderPath = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
        }

        public string FolderPath { get; set; }
    }
}
