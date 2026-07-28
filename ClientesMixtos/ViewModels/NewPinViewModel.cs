using ClientesMixtos.Services;
using ClientesMixtos.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;

namespace ClientesMixtos.ViewModels
{
    public partial class NewPinViewModel(PasswordService passwordService) : ObservableObject
    {
        private readonly PasswordService _passwordService = passwordService;

        [ObservableProperty]
        private string _usuario = string.Empty;

        [RelayCommand]
        public async Task AddPin(object? pin)
        {
            var passwordBox = pin as PasswordBox;
            var pinValue = passwordBox?.Password;

            if (await _passwordService.SavePassword(pinValue ?? "", Usuario))
            {
                MessageBox.Show("PIN guardado correctamente!");
                Application.Current.Windows.OfType<NewPinDialog>().FirstOrDefault()?.Close();

            }
            else
            {
                MessageBox.Show("Error al guardar el PIN: El usuario ya existe o PIN invalido");
            }
        }
    }
}
