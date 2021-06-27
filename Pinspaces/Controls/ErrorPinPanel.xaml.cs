using Pinspaces.Core.Controls;
using Pinspaces.Core.Interfaces;

namespace Pinspaces.Controls
{
    [PinType(DisplayName = "Error Pin", PinType = typeof(ErrorPin))]
    public partial class ErrorPinPanel : ErrorPinUserControl
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

    public abstract class ErrorPinUserControl : PinUserControl<ErrorPin> { }
}
