using System.Windows;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class AddPagoDialog : Window
    {
        public AddPagoDialog(ViewModels.AddPagoViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.CloseRequested += (r) => DialogResult = r;
        }
    }
}
