using ClientesMixtos.Models;
using System.Windows;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class AddClienteDialog : Window
    {
        private readonly ViewModels.AddClienteViewModel _vm;

        public AddClienteDialog(ViewModels.AddClienteViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            _vm.CloseRequested += (r) => DialogResult = r;

            DataContext = vm;
        }

        public Cliente GetCliente() => _vm.Cliente;
    }
}
