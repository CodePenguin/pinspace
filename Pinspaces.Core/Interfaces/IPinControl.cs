using Pinspaces.Core.Data;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Pinspaces.Core.Interfaces
{
    public interface IPinControl : INotifyPropertyChanged
    {
        public Control ContentControl { get; }

        public void AddContextMenuItems(ContextMenu contextMenu);

        public void LoadPin(Guid pinspaceId, Pin pin);
    }
}
