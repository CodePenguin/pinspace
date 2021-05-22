using Pinspaces.Extensions;
using System;
using System.Collections.Generic;

namespace Pinspaces.Data
{
    public class Pinspace : ICloneable
    {
        public Pinspace()
        {
            Id = Guid.NewGuid();
            Pins = new List<Pin>();
        }

        public Guid Id { get; set; }
        public IList<Pin> Pins { get; set; }

        public object Clone()
        {
            var clone = (Pinspace)MemberwiseClone();
            clone.Pins = Pins.Clone();
            return clone;
        }
    }
}
