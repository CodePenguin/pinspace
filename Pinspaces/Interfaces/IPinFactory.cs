using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Pinspaces.Interfaces
{
    public interface IPinFactory
    {
        IEnumerable<Type> PinControlTypes { get; }

        public string GetDisplayName(string pinTypeName);

        public string GetDisplayName(Type pinControlType);

        public Type GetPinType(Type pinControlType);

        public Type GetPinType(string pinTypeName);

        Pin NewPin(Type pinControlType);

        IPinControl NewPinControl(Type pinType);
    }
}
