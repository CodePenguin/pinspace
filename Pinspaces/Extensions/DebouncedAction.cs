using Pinspaces.Core.Interfaces;
using System;
using System.Windows.Threading;

namespace Pinspaces.Extensions
{
    public class DebouncedAction : IDelayedAction
    {
        private readonly Action action;
        private readonly Dispatcher dispatcher;
        private readonly int intervalMilliseconds;
        private bool isPending = true;
        private DispatcherTimer timer = null;

        public DebouncedAction(Action action, int intervalMilliseconds, Dispatcher dispatcher = null)
        {
            this.action = action;
            this.intervalMilliseconds = intervalMilliseconds;
            this.dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        public void Execute()
        {
            timer?.Stop();
            timer = null;

            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(intervalMilliseconds), DispatcherPriority.ApplicationIdle, (s, e) =>
            {
                if (timer == null)
                {
                    return;
                }
                isPending = true;
                timer?.Stop();
                timer = null;
                action.Invoke();
                isPending = false;
            }, dispatcher);

            timer.Start();
        }

        public void Stop(bool executeNowIfPending = true)
        {
            timer?.Stop();
            timer = null;
            if (executeNowIfPending && isPending)
            {
                dispatcher.Invoke(action);
                isPending = false;
            }
        }
    }
}
