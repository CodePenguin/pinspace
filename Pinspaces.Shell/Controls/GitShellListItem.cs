using Pinspaces.Shell.Git;
using System.IO;

namespace Pinspaces.Shell.Controls
{
    public class GitShellListItem : ShellListItem
    {
        private readonly GitStatusEntry gitStatus;

        public GitShellListItem(string filePath, GitStatusEntry gitStatus) : base(filePath)
        {
            this.gitStatus = gitStatus;
        }

        public new string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(gitStatus.FromPath))
                {
                    var fromFilename = Path.GetFileName(gitStatus.FromPath);
                    return $"{base.DisplayName} (from {fromFilename}";
                }
                return base.DisplayName;
            }
        }

        public string Group => gitStatus.IndexStatus == GitStatusCode.Untracked ? "Untracked Files" : "Modified Files";

        public string Status
        {
            get
            {
                var display = StatusCodeToString(gitStatus.IndexStatus);
                if (string.IsNullOrEmpty(display))
                {
                    display = StatusCodeToString(gitStatus.WorkTreeStatus);
                }
                return display;
            }
        }

        private static string StatusCodeToString(GitStatusCode value)
        {
            return value switch
            {
                GitStatusCode.Modified => "Modified",
                GitStatusCode.Added => "Added",
                GitStatusCode.Deleted => "Deleted",
                GitStatusCode.Renamed => "Renamed",
                GitStatusCode.Copied => "Copied",
                GitStatusCode.Untracked => "Untracked",
                _ => ""
            };
        }
    }
}
