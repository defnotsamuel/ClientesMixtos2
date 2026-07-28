using ClientesMixtos.ViewModels;
using System.Windows;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class NewNotaDialog : Window
    {
        public NewNotaDialog(NotaFormViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.CerrarVentana = (resultado) => DialogResult = resultado;
        }
    }
}
