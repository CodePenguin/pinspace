using Pinspace.Config;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Pinspace.PinPanels
{
    [DisplayName("Text Box")]
    internal class TextBoxPinPanel : PinPanel
    {
        private TextBox textBox;

        public TextBoxPinPanel() : base()
        {
        }

        public override PinPanelConfig Config()
        {
            var config = base.Config() as TextBoxPinPanelConfig;
            config.Text = textBox.Text;
            return config;
        }

        public override void LoadConfig(PinPanelConfig config)
        {
            base.LoadConfig(config);
            var typedConfig = config as TextBoxPinPanelConfig;
            textBox.Text = typedConfig.Text;
        }

        protected override Type ConfigType()
        {
            return typeof(TextBoxPinPanelConfig);
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
