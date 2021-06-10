using Pinspaces.Core.Interfaces;
using System;

namespace Pinspaces.Extensions
{
    public class DelayedActionFactory : IDelayedActionFactory
    {
        public IDelayedAction Debounce(Action action, int intervalMilliseconds)
        {
            return new DebouncedAction(action, intervalMilliseconds);
        }
    }
}
