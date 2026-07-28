using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ClientesMixtos.Views.Dialogs
{
    public partial class MarcarDialog : Window
    {
        public MarcarDialog(ViewModels.MarcarDialogViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;

            vm.CloseRequested += (r) => DialogResult = r;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]$");
        }
    }
}
