using ClientesMixtos.ViewModels;
using System.Windows;

namespace ClientesMixtos.Views
{
    public partial class PagosView : Window
    {
        public PagosView(PagosViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
