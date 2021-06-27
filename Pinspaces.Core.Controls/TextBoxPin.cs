using Pinspaces.Core.Data;

namespace Pinspaces.Core.Controls
{
    public class TextBoxPin : Pin
    {
        private string text;
        private bool wordWrap;

        public string Text { get => text; set => SetProperty(ref text, value); }
        public bool WordWrap { get => wordWrap; set => SetProperty(ref wordWrap, value); }
    }
}
