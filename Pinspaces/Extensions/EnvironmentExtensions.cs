using System.IO;
using static System.Environment;

namespace Pinspaces.Extensions
{
    public static class EnvironmentExtensions
    {
        public static string GetLocalApplicationDataFolderPath => Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "Pinspaces");
    }
}
