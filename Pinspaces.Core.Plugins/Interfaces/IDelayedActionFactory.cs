using System;

namespace Pinspaces.Core.Interfaces
{
    public interface IDelayedActionFactory
    {
        public IDelayedAction Debounce(Action action, int intervalMilliseconds);
    }
}
