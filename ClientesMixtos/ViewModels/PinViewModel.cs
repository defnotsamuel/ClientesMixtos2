using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ClientesMixtos.Views.Dialogs;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ClientesMixtos.Services;
using ClientesMixtos.Views;

namespace ClientesMixtos.ViewModels
{
    public partial class PinViewModel(PasswordService passwordService, IServiceProvider serviceProvider) : ObservableObject
    {
        private readonly PasswordService _passwordService = passwordService;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [ObservableProperty]
        private string _usuario = string.Empty;

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
            }
            else
            {
                MessageBox.Show("Bienvenido!");
                OpenMainWindow();
            }
        }

        public void OpenMainWindow()
        {
            var mainView = ActivatorUtilities.CreateInstance<MainWindow>(_serviceProvider);

            Application.Current.MainWindow = mainView;
            Application.Current.Windows.OfType<PinDialog>().FirstOrDefault()?.Close();
            
            mainView.Show();
        }
    }
}
