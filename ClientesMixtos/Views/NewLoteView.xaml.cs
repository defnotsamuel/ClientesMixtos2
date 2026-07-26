using System.Windows;

namespace ClientesMixtos.Views
{
    public partial class NewLoteView : Window
    {
        public NewLoteView(ViewModels.NewLoteViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;

            vm.CloseRequested += ((r) => DialogResult = r);
        }
    }
}
