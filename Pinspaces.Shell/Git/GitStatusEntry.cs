namespace Pinspaces.Shell.Git
{
    public class GitStatusEntry
    {
        public string FromPath { get; set; }
        public GitStatusCode IndexStatus { get; set; }
        public string ToPath { get; set; }
        public GitStatusCode WorkTreeStatus { get; set; }
    }
}
