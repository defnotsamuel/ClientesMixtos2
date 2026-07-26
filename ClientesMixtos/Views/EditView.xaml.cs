using System.Windows;

namespace ClientesMixtos.Views
{
    public partial class EditView : Window
    {

        public EditView(
            ViewModels.EditViewModel editViewModel)
        {

            DataContext = editViewModel;

            editViewModel.CloseRequested += ((r) => DialogResult = r);

            InitializeComponent();
        }
    }
}
