using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Pinspaces.Controls
{
    [PinType(DisplayName = "Error Pin", PinType = typeof(ErrorPin))]
    public partial class ErrorPinPanel : UserControl, IPinControl
    {
        private ErrorPin errorPin;

        public ErrorPinPanel()
        {
            InitializeComponent();
            DataContext = errorPin;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Control ContentControl => this;

        public void AddContextMenuItems(ContextMenu contextMenu)
        {
        }

        public void LoadPin(Guid pinspaceId, Pin pin)
        {
            DataContext = pin;
            errorPin = pin as ErrorPin;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
