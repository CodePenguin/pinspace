using System.ComponentModel;
using System.Timers;
using System.Windows.Threading;

namespace Pinspaces.Extensions
{
    public sealed class DebounceMethodExecutor : Component
    {
        private readonly Dispatcher dispatcher;
        private readonly DebounceMethod method;
        private readonly Timer timer;
        private bool canceled = false;
        private bool pending = false;

        public DebounceMethodExecutor(DebounceMethod method, int waitMilliseconds, Dispatcher dispatcher = null)
        {
            this.dispatcher = dispatcher;
            this.method = method;
            timer = new Timer(waitMilliseconds)
            {
                AutoReset = false
            };
            timer.Elapsed += Timer_Elapsed;
        }

        public delegate void DebounceMethod();

        public void Cancel()
        {
            canceled = true;
            timer.Stop();
        }

        public void Execute()
        {
            timer.Stop();
            pending = true;
            timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Cancel();
                timer.Dispose();
                if (pending)
                {
                    ExecuteMethod();
                    pending = false;
                }
            }
            base.Dispose(disposing);
        }

        private void ExecuteMethod()
        {
            if (dispatcher != null)
            {
                dispatcher.Invoke(method);
            }
            else
            {
                method();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!canceled)
            {
                ExecuteMethod();
                pending = false;
            }
        }
    }
}
