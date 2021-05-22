using Pinspaces.Data;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Pinspaces.Pins
{
    [DisplayName("Text Box")]
    internal class TextBoxPinPanel : PinPanel
    {
        private TextBox textBox;

        private TextBoxPin Pin => pin as TextBoxPin;

        public override void Load(Pin pin)
        {
            base.Load(pin);
            textBox.Text = Pin.Text;
        }

        public override Type PinType()
        {
            return typeof(TextBoxPin);
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
            textBox.TextChanged += TextBox_OnTextChanged;
            Controls.Add(textBox);

            base.InitializeControl();
        }

        private void TextBox_OnTextChanged(object sender, EventArgs e)
        {
            Pin.Text = textBox.Text;
        }
    }
}
