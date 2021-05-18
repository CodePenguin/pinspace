using System.Drawing;
using System.Windows.Forms;

namespace Pinspace.Extensions
{
    internal static class FormExtensions
    {
        public static DialogResult ShowInputDialog(this Form _, string caption, ref string input)
        {
            var size = new Size(200, 70);
            using (var dialog = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                MinimizeBox = false,
                MaximizeBox = false,
                StartPosition = FormStartPosition.CenterParent,
                ShowInTaskbar = false,
                Text = caption
            })
            {
                var textBox = new TextBox
                {
                    Size = new Size(size.Width - 10, 23),
                    Location = new Point(5, 5),
                    Text = input
                };
                dialog.Controls.Add(textBox);

                var okButton = new Button
                {
                    DialogResult = DialogResult.OK,
                    Name = "okButton",
                    Size = new Size(75, 23),
                    Text = "&OK",
                    Location = new Point(size.Width - 80 - 80, 39)
                };
                dialog.Controls.Add(okButton);

                var cancelButton = new Button
                {
                    DialogResult = DialogResult.Cancel,
                    Name = "cancelButton",
                    Size = new Size(75, 23),
                    Text = "&Cancel",
                    Location = new Point(size.Width - 80, 39)
                };
                dialog.Controls.Add(cancelButton);

                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                var result = dialog.ShowDialog();
                input = textBox.Text;
                return result;
            }
        }
    }
}
