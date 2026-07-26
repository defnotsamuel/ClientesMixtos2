using System.Windows;

namespace ClientesMixtos.Views
{
    public partial class AddPagoView : Window
    {
        public AddPagoView(ViewModels.AddPagoViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.CloseRequested += (r) => DialogResult = r;
        }
    }
}
