using Pinspaces.Core.Data;
using System.Collections.Generic;

namespace Pinspaces.Shell.Controls
{
    public class FileListPin : Pin
    {
        private List<string> files = new();

        public List<string> Files { get => files; set => SetProperty(ref files, value); }

        public void NotifyFilesChanged()
        {
            NotifyPropertyChanged(nameof(Files));
        }
    }
}
