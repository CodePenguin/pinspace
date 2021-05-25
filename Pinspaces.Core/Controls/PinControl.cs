using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.Windows.Forms;

namespace Pinspaces.Core.Controls
{
    public abstract class PinControl : Panel, INotifyPropertiesChanged
    {
        protected Pin pin;

        public PinControl()
        {
            InitializeControl();
        }

        public event INotifyPropertiesChanged.PropertiesChangedEventHandler PropertiesChanged;

        public virtual void AddContextMenuItems(ContextMenuStrip contextMenu)
        {
            // Override in descendent to add custom menu items
        }

        public virtual void LoadPin(Pin pin)
        {
            this.pin = pin;
        }

        protected virtual void InitializeControl()
        {
        }

        protected void SendPropertiesChangedNotification()
        {
            PropertiesChanged?.Invoke(this, new EventArgs());
        }
    }
}
