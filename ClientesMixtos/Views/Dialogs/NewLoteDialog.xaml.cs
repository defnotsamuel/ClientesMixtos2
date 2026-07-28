using System.Windows;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class NewLoteDialog : Window
    {
        public NewLoteDialog(ViewModels.NewLoteViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;

            vm.CloseRequested += ((r) => DialogResult = r);
        }
    }
}
