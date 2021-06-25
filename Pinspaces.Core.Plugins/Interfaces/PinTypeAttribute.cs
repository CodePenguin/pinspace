using System;

namespace Pinspaces.Core.Interfaces
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PinTypeAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public Type PinType { get; set; }
    }
}
