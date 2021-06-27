using Pinspaces.Core.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Pinspaces.Core.Controls
{
    [PinType(DisplayName = "Text Box", PinType = typeof(TextBoxPin))]
    public partial class TextBoxPinPanel : TextBoxPinUserControl
    {
        public TextBoxPinPanel()
        {
            InitializeComponent();
        }

        public override void AddContextMenuItems(ContextMenu contextMenu)
        {
            var menuItem = new MenuItem { Header = "Word Wrap", IsChecked = textBox.TextWrapping == TextWrapping.Wrap };
            menuItem.Click += WordWrapContextMenuItem_Click;
            contextMenu.Items.Add(menuItem);
        }

        protected override void LoadPin()
        {
            DataContext = Pin;
            textBox.Text = Pin.Text;
            textBox.TextWrapping = Pin.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Pin.Text = textBox.Text;
            NotifyPinPropertyChanged(nameof(Pin.Text));
        }

        private void WordWrapContextMenuItem_Click(object sender, EventArgs e)
        {
            Pin.WordWrap = !Pin.WordWrap;
            textBox.TextWrapping = Pin.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            NotifyPinPropertyChanged(nameof(Pin.WordWrap));
        }
    }

    public abstract class TextBoxPinUserControl : PinUserControl<TextBoxPin> { }
}
