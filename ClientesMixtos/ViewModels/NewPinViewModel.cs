using ClientesMixtos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;

namespace ClientesMixtos.ViewModels
{
    public partial class NewPinViewModel : DialogViewModelBase
    {
        private readonly IPasswordService _passwordService;

        [ObservableProperty]
        private string _usuario = string.Empty;

        public NewPinViewModel(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [RelayCommand]
        public async Task AddPin(object? pin)
        {
            var passwordBox = pin as PasswordBox;
            var pinValue = passwordBox?.Password;

            if (await _passwordService.SavePassword(pinValue ?? "", Usuario))
            {
                MessageBox.Show("PIN guardado correctamente!");
                RequestClose(true);
            }
            else
            {
                MessageBox.Show("Error al guardar el PIN: El usuario ya existe o PIN invalido");
            }
        }
    }
}
