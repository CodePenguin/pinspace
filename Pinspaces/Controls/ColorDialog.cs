using System.Windows.Forms;
using System.Windows.Media;

namespace Pinspaces.Controls
{
    public class ColorDialog
    {
        public static bool ShowDialog(ref Color color)
        {
            using var colorDialog = new System.Windows.Forms.ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B),
                FullOpen = true
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                color = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                return true;
            }
            return false;
        }
    }
}
