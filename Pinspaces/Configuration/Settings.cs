using Pinspaces.Extensions;
using System.Collections.Generic;

namespace Pinspaces.Configuration
{
    public class Settings
    {
        public List<ApplicationAction> Actions { get; set; } = new();
        public string DefaultPinColor { get; set; } = "#337ab7";
        public string DefaultPinspaceColor { get; set; } = "#b0d8ff";
        public string LocalApplicationDataFolderPath { get; set; } = EnvironmentExtensions.GetLocalApplicationDataFolderPath;
    }
}
