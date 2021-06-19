using Pinspaces.Extensions;
using System.Collections.Generic;
using System.Windows.Media;

namespace Pinspaces.Configuration
{
    public class Settings
    {
        public List<ApplicationAction> Actions { get; set; } = new();
        public string DefaultPinColor { get; set; }
        public string DefaultPinspaceColor { get; set; }
        public string LocalApplicationDataFolderPath { get; set; } = EnvironmentExtensions.GetLocalApplicationDataFolderPath;

        public Color GetDefaultPinColor()
        {
            return ColorExtensions.FromHtmlString(DefaultPinColor, Color.FromArgb(255, 51, 122, 183));
        }

        public Color GetDefaultPinspaceColor()
        {
            return ColorExtensions.FromHtmlString(DefaultPinspaceColor, Color.FromArgb(255, 176, 216, 255));
        }
    }
}
