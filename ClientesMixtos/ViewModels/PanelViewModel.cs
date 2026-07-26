using ClientesMixtos.Models;
using ClientesMixtos.Services;
using ClientesMixtos.Views;
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
using System.Windows.Input;

namespace ClientesMixtos.ViewModels
{
    public partial class PanelViewModel: ObservableObject
    {
        private readonly ClienteService _clienteService;
        private readonly ObservableCollection<Cliente> _pendingClientes;

        [ObservableProperty]
        private int _totalClientes;

        [ObservableProperty]
        private int _totalPendingClientes;

        [ObservableProperty]
        private int _totalPerdidos;

        [ObservableProperty]
        private int _totalRecuperados;

        [ObservableProperty]
        private string _textoBusqueda = string.Empty;

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

        [RelayCommand]
        public async Task Mark(Cliente cliente)
        {
            if (cliente.State.FechaDePago == null)
            {
                MessageBox.Show("El cliente no tiene una fecha de pago!", "ERROR CLIENTE", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialogVm = new MarcarDialogViewModel();
            var dialog = new MarcarDialogView(dialogVm);
            if (dialog.ShowDialog() != true) return;

            await _clienteService.MarcarCliente(cliente, dialogVm.Meses);
            PendingClientesView.Refresh();

            TotalPendingClientes--;
        }

        partial void OnTextoBusquedaChanged(string value) => PendingClientesView.Filter = (obj) =>
        {
            if (obj is Cliente cliente)
            {
                return string.IsNullOrEmpty(TextoBusqueda) ||
                       cliente.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                       cliente.Vehiculo.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };

        public PanelViewModel(ClienteService service)
        {
            _clienteService = service;
            _pendingClientes = [];
            PendingClientesView = CollectionViewSource.GetDefaultView(_pendingClientes);
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var clientes = await _clienteService.GetAll();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    TotalClientes = clientes.Count;

                    foreach (var c in FilterClientsByCurrentMonth(clientes))
                    {
                        _pendingClientes.Add(c);
                    }

                    TotalPendingClientes = _pendingClientes.Count(c => !c.State.MarcadoEsteMes);
                    TotalPerdidos = clientes.Count(c => c.Perdido);
                    TotalRecuperados = clientes.Count(c => c.Recuperado);
                    PendingClientesView.Refresh();
                });
            }
            catch
            {
                // BD desconectada — los datos se cargarán al reconectar
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
