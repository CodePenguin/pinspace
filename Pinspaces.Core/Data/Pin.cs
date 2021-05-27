using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using System;

namespace Pinspaces.Core.Data
{
    public abstract class Pin : ICloneable<Pin>
    {
        public Pin()
        {
            Id = Guid.NewGuid();
        }

        public string Color { get; set; }
        public int Height { get; set; } = 200;
        public Guid Id { get; set; }
        public int Left { get; set; }
        public string Title { get; set; }
        public int Top { get; set; }
        public int Width { get; set; } = 300;

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
