using System;

namespace Pinspaces.Core.Interfaces
{
    public interface INotifyPropertiesChanged
    {
        public delegate void PropertiesChangedEventHandler(object sender, EventArgs e);

        event PropertiesChangedEventHandler PropertiesChanged;
    }
}
