namespace Pinspaces.Core.Interfaces
{
    public interface IDelayedAction
    {
        public void Execute();

        public void Stop(bool executeNowIfPending = true);
    }
}
