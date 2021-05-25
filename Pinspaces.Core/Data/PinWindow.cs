using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using System;

namespace Pinspaces.Core.Data
{
    public class PinWindow : ICloneable<PinWindow>
    {
        public const int DefaultHeight = 600;

        public const int DefaultWidth = 800;

        public PinWindow()
        {
            Id = Guid.NewGuid();
        }

        public Guid ActivePinspaceId { get; set; }

        public int Height { get; set; }

        public Guid Id { get; set; }

        public bool IsMaximized { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public int Width { get; set; }

        public void Assign(PinWindow source, out bool wasChanged)
        {
            CloneExtensions.Assign(GetType(), this, source, out wasChanged);
        }

        public PinWindow Clone()
        {
            var clone = (PinWindow)MemberwiseClone();
            Assign(this, out _);
            return clone;
        }
    }
}
