using ClientesMixtos.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private string _connectionStatus = "Verificando...";

        [ObservableProperty]
        private string _connectionStatusColor = "#6B7280";

        private readonly IMongoContext _context;
        private readonly IServiceProvider _serviceProvider;

        [RelayCommand]
        public void ShowPanel()
        {
            var vm = _serviceProvider.GetRequiredService<PanelViewModel>();
            CurrentView = vm;
        }

        [RelayCommand]
        public void ShowClientes()
        {
            var vm = _serviceProvider.GetRequiredService<ClientesViewModel>();
            CurrentView = vm;
        }

        [RelayCommand]
        public void ShowConfig()
        {
            CurrentView = _serviceProvider.GetRequiredService<ConfigViewModel>();
        }

        [RelayCommand]
        public async Task RefreshConnection()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ConnectionStatus = "Verificando...";
                ConnectionStatusColor = "#6B7280";
            });
            await CheckConnectionAsync();
        }

        public async Task CheckConnectionAsync()
        {
            try
            {
                await _context.Database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ConnectionStatus = "Conectado";
                    ConnectionStatusColor = "#10B981";
                });
            }
            catch
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ConnectionStatus = "Desconectado";
                    ConnectionStatusColor = "#EF4444";
                });
            }
        }

        public MainViewModel(IMongoContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;

            var panelVm = _serviceProvider.GetRequiredService<PanelViewModel>();
            CurrentView = panelVm;

            _ = CheckConnectionAsync();
        }
    }
}
