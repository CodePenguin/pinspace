using Pinspaces.Core.Data;
using System;
using System.IO;

namespace Pinspaces.Shell.Controls
{
    public class GitFolderViewPin : Pin
    {
        public GitFolderViewPin()
        {
            RepositoryPath = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
        }

        public string RepositoryPath { get; set; }
    }
}
