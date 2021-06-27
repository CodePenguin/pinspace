using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.Windows.Controls;

namespace Pinspaces.Core.Controls
{
    public abstract partial class PinUserControl : UserControl { }

    public abstract class PinUserControl<TPin> : PinUserControl, IPinControl where TPin : Pin
    {
        public virtual Control ContentControl => this;
        public TPin Pin { get; private set; }
        public Guid PinspaceId { get; private set; }

        public virtual void AddContextMenuItems(ContextMenu contextMenu)
        {
        }

        public void LoadPin(Guid pinspaceId, Pin pin)
        {
            PinspaceId = pinspaceId;
            Pin = pin as TPin;
            LoadPin();
        }

        protected virtual void LoadPin()
        {
        }
    }
}
