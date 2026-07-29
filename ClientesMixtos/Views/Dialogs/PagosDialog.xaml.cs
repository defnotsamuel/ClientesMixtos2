using ClientesMixtos.ViewModels;
using System.Windows;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class PagosDialog : Window
    {
        private readonly PagosViewModel _vm;

        public PagosDialog(PagosViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
            _vm = vm;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
