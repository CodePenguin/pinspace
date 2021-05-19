using System.Collections.Generic;

namespace Pinspace.Config
{
    public class PinspaceConfig
    {
        public PinspaceConfig()
        {
            Windows = new List<PinWindowConfig>();
        }

        public List<PinWindowConfig> Windows { get; set; }
    }
}
