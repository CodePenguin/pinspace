using System.Windows.Forms;

namespace FilePinboard
{
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
