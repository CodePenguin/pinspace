using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;

namespace Pinspaces.Data
{
    public abstract class Pin : ICloneable<Pin>
    {
        public string Color { get; set; }
        public int Height { get; set; }
        public int Left { get; set; }
        public string Title { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }

        public virtual void Assign(Pin source, out bool wasChanged)
        {
            CloneExtensions.Assign(GetType(), this, source, out wasChanged);
        }

        public Pin Clone()
        {
            var clone = (Pin)MemberwiseClone();
            clone.Assign(this, out _);
            return clone;
        }
    }
}
