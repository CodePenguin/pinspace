using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
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

        PinControl NewPinControl(Type pinType);
    }
}
