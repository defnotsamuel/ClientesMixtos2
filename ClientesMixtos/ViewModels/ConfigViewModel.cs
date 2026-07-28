using ClientesMixtos.Configuration;
using ClientesMixtos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ClientesMixtos.Views.Dialogs;

namespace ClientesMixtos.ViewModels
{
    public partial class ConfigViewModel(Services.PasswordService service) : ObservableObject
    {
        private readonly Services.PasswordService _passwordService = service;

        [ObservableProperty]
        private string _connectionString = GlobalConfig.ConnectionString();

        [ObservableProperty]
        private string _databaseName = GlobalConfig.DatabaseName();

        [ObservableProperty]
        private bool _isDarkMode = GlobalConfig.Theme() == "Dark";

        partial void OnIsDarkModeChanged(bool value)
        {
            var theme = value ? "Dark" : "Light";
            GlobalConfig.SetTheme(theme);
            GlobalConfig.SaveConfig();
            ThemeManager.Apply(theme);
        }

        [RelayCommand]
        public void ConfigurePIN()
        {
            var newPinView = new NewPinDialog();
            var model = new NewPinViewModel(_passwordService);

            newPinView.DataContext = model;
            newPinView.ShowDialog();
        }

        [RelayCommand]
        public void SaveConfig()
        {
            GlobalConfig.SetConnectionString(ConnectionString);
            GlobalConfig.SetDatabaseName(DatabaseName);
            GlobalConfig.SaveConfig();

            MessageBox.Show(
                "Configuración guardada. Reinicia la aplicación para aplicar los cambios.",
                "Configuración",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
