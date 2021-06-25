using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using System;

namespace Pinspaces.Core.Data
{
    public class Pinspace : ICloneable<Pinspace>
    {
        public Pinspace()
        {
            Id = Guid.NewGuid();
        }

        public string Color { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }

        public void Assign(Pinspace source, out bool wasChanged)
        {
            CloneExtensions.Assign(GetType(), this, source, out var pinspaceWasChanged);
            wasChanged = pinspaceWasChanged;
        }

        public Pinspace Clone()
        {
            var clone = (Pinspace)MemberwiseClone();
            Assign(this, out _);
            return clone;
        }
    }
}
