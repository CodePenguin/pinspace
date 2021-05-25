using System;

namespace Pinspaces.Core.Controls
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PinTypeAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public Type PinType { get; set; }
    }
}
