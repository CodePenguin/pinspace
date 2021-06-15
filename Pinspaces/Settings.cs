using Pinspaces.Extensions;

namespace Pinspaces
{
    public class Settings
    {
        public string DefaultPinColor { get; set; } = "#337ab7";
        public string DefaultPinspaceColor { get; set; } = "#b0d8ff";
        public string LocalApplicationDataFolderPath { get; set; } = EnvironmentExtensions.GetLocalApplicationDataFolderPath;
    }
}
