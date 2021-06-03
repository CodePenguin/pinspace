using Pinspaces.Core.Data;
using System.ComponentModel;
using System.Windows.Controls;

namespace Pinspaces.Core.Interfaces
{
    public interface IPinControl : INotifyPropertyChanged
    {
        public Control ContentControl { get; }

        public void AddContextMenuItems(ContextMenu contextMenu);

        public void LoadPin(Pin pin);
    }
}
