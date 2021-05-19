using System.Collections.Generic;

namespace Pinspace.Config
{
    public class FileListPinPanelConfig : PinPanelConfig
    {
        public FileListPinPanelConfig()
        {
            Files = new List<string>();
        }

        public List<string> Files { get; set; }
    }
}
