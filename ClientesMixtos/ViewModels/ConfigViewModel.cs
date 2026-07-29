using ClientesMixtos.Configuration;
using ClientesMixtos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ClientesMixtos.Views.Dialogs;

namespace ClientesMixtos.ViewModels
{
    public partial class ConfigViewModel : ObservableObject
    {
        private readonly GlobalConfig _config;
        private readonly UIFactory _uiFactory;

        [ObservableProperty]
        private string _connectionString;

        [ObservableProperty]
        private string _databaseName;

        public ConfigViewModel(GlobalConfig config, UIFactory uiFactory)
        {
            _config = config;
            _uiFactory = uiFactory;
            _connectionString = config.ConnectionString;
            _databaseName = config.DatabaseName;
        }

        [RelayCommand]
        public void ConfigurePIN()
        {
            var newPinView = _uiFactory.Create<NewPinDialog>();
            newPinView.ShowDialog();
        }

        [RelayCommand]
        public void SaveConfig()
        {
            _config.SetConnectionString(ConnectionString);
            _config.SetDatabaseName(DatabaseName);
            _config.Save();

            MessageBox.Show(
                "Configuración guardada. Reinicia la aplicación para aplicar los cambios.",
                "Configuración",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
