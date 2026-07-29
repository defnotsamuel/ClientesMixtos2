using ClientesMixtos.Models;
using ClientesMixtos.Services;
using ClientesMixtos.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ClientesMixtos.ViewModels
{
    public partial class PanelViewModel : ObservableObject
    {
        private readonly IClienteService _clienteService;
        private ObservableCollection<Cliente> _pendingClientes = [];

        [ObservableProperty]
        private int _totalClientes;

        [ObservableProperty]
        private int _pendingClientesCount;

        [ObservableProperty]
        private int _totalPerdidos;

        [ObservableProperty]
        private int _totalRecuperados;

        [ObservableProperty]
        private string _searchText = string.Empty;

        public static string CurrentMonth
        {
            get
            {
                var cultura = new CultureInfo("es-SV");
                return cultura.TextInfo.ToTitleCase(
                    DateTime.Now.ToString("MMMM", cultura)
                );
            }
        }

        public ICollectionView PendingClientesView { get; set; }

        public PanelViewModel(IClienteService service)
        {
            _clienteService = service;
            PendingClientesView = CollectionViewSource.GetDefaultView(_pendingClientes);

            _ = LoadDataAsync();
        }

        [RelayCommand]
        public async Task Mark(Cliente cliente)
        {
            if (cliente.State.FechaDePago == null)
            {
                MessageBox.Show("El cliente no tiene una fecha de pago!", "ERROR CLIENTE", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialogVm = new MarcarDialogViewModel();
            var dialog = new MarcarDialog(dialogVm);
            if (dialog.ShowDialog() != true) return;

            await _clienteService.MarcarCliente(cliente, dialogVm.Meses);

            PendingClientesView.Refresh();
            PendingClientesCount--;
        }

        partial void OnSearchTextChanged(string value) => PendingClientesView.Filter = (obj) =>
        {
            if (obj is Cliente cliente)
            {
                return string.IsNullOrEmpty(SearchText) ||
                       cliente.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       cliente.Vehiculo.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };

        public async Task LoadDataAsync()
        {
            try
            {
                var clientes = await _clienteService.GetAll();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var clientesPendientes = FilterClientsByCurrentMonth(clientes);
                    foreach (var cliente in clientesPendientes)
                        _pendingClientes.Add(cliente);

                    TotalClientes = clientes.Count;
                    PendingClientesCount = _pendingClientes.Count(c => !c.State.MarcadoEsteMes);
                    TotalPerdidos = clientes.Count(c => c.Perdido);
                    TotalRecuperados = clientes.Count(c => c.Recuperado);
                });
            }
            catch
            {
            }
        }

        private static List<Cliente> FilterClientsByCurrentMonth(List<Cliente> clientes)
        {
            var current = DateTime.Now;

            return [.. clientes.Where(c => (c.State.FechaDePago?.Month == current.Month
                    && c.State.FechaDePago?.Year == current.Year) || c.State.MarcadoEsteMes)];
        }
    }
}
