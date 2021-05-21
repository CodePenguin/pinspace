using System.Collections.Generic;

namespace Pinspace.Config
{
    public class PinWindowConfig
    {
        public PinWindowConfig()
        {
            Panels = new List<PinPanelConfig>();
        }

        public string Color { get; set; }
        public List<PinPanelConfig> Panels { get; set; }
    }
}
