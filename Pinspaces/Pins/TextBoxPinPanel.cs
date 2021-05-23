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

        public override void AddContextMenuItems(ContextMenuStrip contextMenu)
        {
            base.AddContextMenuItems(contextMenu);

            var menuItem = new ToolStripMenuItem { Text = "Word Wrap", Checked = textBox.WordWrap };
            menuItem.Click += WordWrapContextMenuItem_Click;
            contextMenu.Items.Add(menuItem);
        }

        public override void LoadPin(Pin pin)
        {
            base.LoadPin(pin);
            textBox.Text = Pin.Text;
            textBox.WordWrap = Pin.WordWrap;
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
            SendPropertiesChangedNotification();
        }

        private void WordWrapContextMenuItem_Click(object sender, EventArgs e)
        {
            textBox.WordWrap = !textBox.WordWrap;
            Pin.WordWrap = textBox.WordWrap;
            SendPropertiesChangedNotification();
        }
    }
}
