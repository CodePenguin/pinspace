using Pinspaces.Core.Data;
using System;
using System.IO;

namespace Pinspaces.Shell.Controls
{
    public class GitFolderViewPin : Pin
    {
        private string repositoryPath = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

        public string RepositoryPath { get => repositoryPath; set => SetProperty(ref repositoryPath, value); }
    }
}
