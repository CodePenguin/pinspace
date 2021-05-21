using System.Drawing;

namespace Pinspace.Config
{
    public abstract class PinPanelConfig
    {
        public string Color { get; set; }
        public int Height { get; set; }
        public int Left { get; set; }
        public string Title { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
    }
}
