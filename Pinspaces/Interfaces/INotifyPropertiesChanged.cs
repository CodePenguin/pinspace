using System;

namespace Pinspaces.Interfaces
{
    public interface INotifyPropertiesChanged
    {
        public delegate void PropertiesChangedEventHandler(object sender, EventArgs e);

        event PropertiesChangedEventHandler PropertiesChanged;
    }
}
