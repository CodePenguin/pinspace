using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Pinspaces.Core.Data
{
    public class Pinspace : ICloneable<Pinspace>
    {
        public Pinspace()
        {
            Id = Guid.NewGuid();
            Pins = new List<Pin>();
        }

        public string Color { get; set; }
        public Guid Id { get; set; }
        public IList<Pin> Pins { get; set; }

        public void Assign(Pinspace source, out bool wasChanged)
        {
            CloneExtensions.Assign(GetType(), this, source, out var pinspaceWasChanged);
            Pins.Assign(source.Pins, out var pinsWereChanged);
            wasChanged = pinspaceWasChanged || pinsWereChanged;
        }

        public Pinspace Clone()
        {
            var clone = (Pinspace)MemberwiseClone();
            Assign(this, out _);
            return clone;
        }
    }
}
