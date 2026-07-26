using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ClientesMixtos.Services;
using ClientesMixtos.Views;

namespace ClientesMixtos.ViewModels
{
    public partial class PinViewModel : ObservableObject
    {
        private readonly PasswordService _passwordService;
        private readonly IServiceProvider _serviceProvider;

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

                var mainView = new MainView
                {
                    DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
                };

                Application.Current.MainWindow = mainView;
                mainView.Show();

                Application.Current.Windows.OfType<PinView>().FirstOrDefault()?.Close();
            }
        }

        public PinViewModel(PasswordService passwordService, IServiceProvider serviceProvider)
        {
            _passwordService = passwordService;
            _serviceProvider = serviceProvider;
        }
    }
}
