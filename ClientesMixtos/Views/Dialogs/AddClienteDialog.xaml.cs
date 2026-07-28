using ClientesMixtos.Models;
using System.Windows;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class AddClienteDialog : Window
    {

        private ViewModels.AddClienteViewModel vm;

        public AddClienteDialog(ViewModels.AddClienteViewModel vm)
        {
            InitializeComponent();

            this.vm = vm;
            vm.CloseRequested += ((r) => DialogResult = r);

            DataContext = vm;
        }

        public Cliente GetCliente() => vm.Cliente;
    }
}
