using System.ComponentModel;
using System.Windows.Forms;

namespace Pinspace
{
    [DisplayName("Text Box")]
    internal class TextBoxCell : PinboardCell
    {
        private TextBox textBox;

        public TextBoxCell() : base()
        {
        }

        protected override void InitializeControl()
        {
            textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false
            };
            Controls.Add(textBox);

            base.InitializeControl();
        }
    }
}
