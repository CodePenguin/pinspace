using System.Windows;

namespace Pinspaces.Controls
{
    public partial class InputDialog : Window
    {
        public InputDialog(string title, string prompt, string defaultValue = "")
        {
            InitializeComponent();
            Title = title;
            PromptLabel.Content = prompt;
            ValueTextBox.Text = defaultValue;
        }

        public string Value => ValueTextBox.Text;

        public static bool ShowInputDialog(string title, string prompt, ref string value)
        {
            var dialog = new InputDialog(title, prompt, value);
            if (dialog.ShowDialog() == true)
            {
                value = dialog.Value;
                return true;
            }
            return false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            ValueTextBox.SelectAll();
            ValueTextBox.Focus();
        }
    }
}
