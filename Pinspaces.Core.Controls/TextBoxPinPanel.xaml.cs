using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Pinspaces.Core.Controls
{
    [PinType(DisplayName = "Text Box", PinType = typeof(TextBoxPin))]
    public partial class TextBoxPinPanel : UserControl, IPinControl
    {
        private TextBoxPin textBoxPin;

        public TextBoxPinPanel()
        {
            InitializeComponent();
            DataContext = textBoxPin;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Control ContentControl => this;

        public void AddContextMenuItems(ContextMenu contextMenu)
        {
            var menuItem = new MenuItem { Header = "Word Wrap", IsChecked = textBox.TextWrapping == TextWrapping.Wrap };
            menuItem.Click += WordWrapContextMenuItem_Click;
            contextMenu.Items.Add(menuItem);
        }

        public void LoadPin(Pin pin)
        {
            DataContext = pin;
            textBoxPin = pin as TextBoxPin;
            textBox.Text = textBoxPin.Text;
            textBox.TextWrapping = textBoxPin.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxPin.Text = textBox.Text;
            PropertyChanged?.Invoke(textBoxPin, new PropertyChangedEventArgs(nameof(textBoxPin.Text)));
        }

        private void WordWrapContextMenuItem_Click(object sender, EventArgs e)
        {
            textBoxPin.WordWrap = !textBoxPin.WordWrap;
            textBox.TextWrapping = textBoxPin.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            PropertyChanged?.Invoke(textBoxPin, new PropertyChangedEventArgs(nameof(textBoxPin.WordWrap)));
        }
    }
}
