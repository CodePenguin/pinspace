using Pinspaces.Core.Data;
using System;
using System.Windows.Controls;

namespace Pinspaces.Core.Interfaces
{
    public interface IPinControl
    {
        public Control ContentControl { get; }

        public void AddContextMenuItems(ContextMenu contextMenu);

        public void LoadPin(Guid pinspaceId, Pin pin);
    }
}
