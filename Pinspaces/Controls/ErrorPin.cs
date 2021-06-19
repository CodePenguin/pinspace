using Pinspaces.Core.Data;

namespace Pinspaces.Controls
{
    public class ErrorPin : Pin
    {
        public ErrorPin() : base()
        {
            Color = "#f5c6cb";
        }

        public string ErrorMessage { get; set; }
    }
}
