using Autofac;
using System.Windows.Forms;

namespace FilePinboard
{
    public class FormFactory
    {
        private readonly ILifetimeScope scope;

        public FormFactory(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public TForm CreateForm<TForm>() where TForm : Form
        {
            var formScope = scope.BeginLifetimeScope();
            var form = formScope.Resolve<TForm>();

            form.Disposed += (s, e) =>
            {
                formScope.Dispose();
            };

            return form;
        }
    }
}
