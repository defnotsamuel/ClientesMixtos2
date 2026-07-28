using System.Windows;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class EditClienteDialog : Window
    {

        public EditClienteDialog(
            ViewModels.EditViewModel editViewModel)
        {

            DataContext = editViewModel;

            editViewModel.CloseRequested += ((r) => DialogResult = r);

            InitializeComponent();
        }
    }
}
