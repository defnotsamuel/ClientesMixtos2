using ClientesMixtos.ViewModels;
using System.Windows;

namespace ClientesMixtos.Views
{
    public partial class NotaFormWindow : Window
    {
        public NotaFormWindow(NotaFormViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.CerrarVentana = (resultado) => DialogResult = resultado;
        }
    }
}
