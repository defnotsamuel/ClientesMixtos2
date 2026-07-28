using ClientesMixtos.ViewModels;
using System.Windows;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class PagosDialog : Window
    {
        public PagosViewModel vm;
        public PagosDialog(PagosViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
            this.vm = vm;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
