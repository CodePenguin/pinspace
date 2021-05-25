using Pinspaces.Core.Data;
using System.Collections.Generic;

namespace Pinspaces.Shell.Controls
{
    public class FileListPin : Pin
    {
        public FileListPin()
        {
            Files = new List<string>();
        }

        public List<string> Files { get; set; }
    }
}
