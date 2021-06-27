using Pinspaces.Core.Interfaces;
using Pinspaces.Core.Controls;

namespace Pinspaces.Controls
{
    [PinType(DisplayName = "Error Pin", PinType = typeof(ErrorPin))]
    public partial class ErrorPinPanel : PinUserControl<ErrorPin>
    {
        public ErrorPinPanel()
        {
            InitializeComponent();
        }

        protected override void LoadPin()
        {
            DataContext = Pin;
        }
    }
}
