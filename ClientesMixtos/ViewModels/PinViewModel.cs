using ClientesMixtos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClientesMixtos.ViewModels
{
    public partial class PinViewModel : DialogViewModelBase
    {
        private readonly IPasswordService _passwordService;

        [ObservableProperty]
        private string _usuario = string.Empty;

        public PinViewModel(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [RelayCommand]
        public async Task CheckPin(PasswordBox passwordBox)
        {
            var pinValue = passwordBox?.Password;

            if (!await _passwordService.ExistsUser(Usuario))
            {
                MessageBox.Show("El usuario no existe!");
                return;
            }

            if (!await _passwordService.VerifyPassword(pinValue ?? "", Usuario))
            {
                MessageBox.Show("El PIN es invalido!");
                return;
            }

            MessageBox.Show("Bienvenido!");
            RequestClose(true);
        }
    }
}
