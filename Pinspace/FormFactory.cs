using Microsoft.Extensions.DependencyInjection;
using System.Windows.Forms;

namespace Pinspace
{
    public class FormFactory
    {
        private readonly IServiceScopeFactory scopeFactory;

        public FormFactory(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public TForm CreateForm<TForm>() where TForm : Form
        {
            var formScope = scopeFactory.CreateScope();
            var form = formScope.ServiceProvider.GetService<TForm>();

            form.Disposed += (s, e) =>
            {
                formScope.Dispose();
            };

            return form;
        }
    }
}
