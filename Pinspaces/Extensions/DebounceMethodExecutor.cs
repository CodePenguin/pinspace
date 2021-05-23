using System.ComponentModel;
using System.Timers;

namespace Pinspaces.Extensions
{
    public sealed class DebounceMethodExecutor : Component
    {
        private readonly DebounceMethod method;
        private readonly Timer timer;
        private bool canceled = false;
        private bool pending = false;

        public DebounceMethodExecutor(DebounceMethod method, int waitMilliseconds)
        {
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
                    method();
                    pending = false;
                }
            }
            base.Dispose(disposing);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!canceled)
            {
                method();
                pending = false;
            }
        }
    }
}
