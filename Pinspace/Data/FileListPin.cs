using System.Collections.Generic;

namespace Pinspace.Data
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
