using System.Windows;

namespace ClientesMixtos.Views
{
    public partial class AddClienteView : Window
    {
        public AddClienteView(ViewModels.AddClienteViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.CloseRequested += ((r) => DialogResult = r);
        }
    }
}
