using Pinspaces.Core.Data;

namespace Pinspaces.Core.Controls
{
    public class RichTextBoxPin : Pin
    {
        private string content;

        public string Content { get => content; set => SetProperty(ref content, value); }
    }
}
