using ClientesMixtos.Models;
using ClientesMixtos.Repos;
using ClientesMixtos.Services;
using ClientesMixtos.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ClientesMixtos.ViewModels
{
    public partial class ClientesViewModel : ObservableObject
    {
        private readonly ClienteService _clienteService;
        private readonly NotaService _notaService;
        private readonly PagoService _pagoService;

        public ObservableCollection<Cliente> _clientes = [];
        public ObservableCollection<string> Lotes { get; } = [];
        public ICollectionView CClientesView { get; set; }

        [ObservableProperty]
        private string _loteSeleccionado = "Todos";

        [ObservableProperty]
        private string _textoBusqueda = string.Empty;

        [ObservableProperty]
        private Cliente? _clienteSeleccionado;

        public bool HasSelectedClient => ClienteSeleccionado != null;

        partial void OnClienteSeleccionadoChanged(Cliente? value)
        {
            OnPropertyChanged(nameof(HasSelectedClient));
        }

        [RelayCommand]
        public void NewLote(object? target)
        {
            var lotes = new ObservableCollection<string>(Lotes);
            lotes.Remove("Todos");

            var model = new NewLoteViewModel(lotes);
            var newLoteView = new NewLoteDialog(model);
            if (newLoteView.ShowDialog() ?? false)
            {
                Lotes.Add(model.Lote);
                OnPropertyChanged(nameof(Lotes));
            }
        }

        [RelayCommand]
        public async Task Add()
        {
            var lotes = new ObservableCollection<string>(Lotes);
            lotes.Remove("Todos");

            var model = new AddClienteViewModel(lotes);
            var addView = new AddClienteDialog(model);

            if (addView.ShowDialog() ?? false)
            {
                var cliente = model.Cliente;

                await _clienteService.AddCliente(cliente);

                _clientes.Add(cliente);
            }
        }

        [RelayCommand]
        public async Task Delete(Cliente cliente)
        {
            if (MessageBoxResult.Yes != MessageBox.Show($"¿Está seguro de eliminar al cliente {cliente.Nombre}?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning))
            {
                return;
            }

            await _notaService.DeleteByClienteId(cliente.ClienteId);
            await _clienteService.DeleteCliente(cliente);

            _clientes.Remove(cliente);
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
            CClientesView.Refresh();
        }

        [RelayCommand]
        public async Task MarkAsPaid(Cliente cliente)
        {
            if (MessageBoxResult.Yes != MessageBox.Show($"¿Está seguro de saldar al cliente {cliente.Nombre}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning))
            {
                return;
            }

            cliente.Saldado = true;

            await _clienteService.UpdateCliente(cliente);
            CClientesView.Refresh();
        }

        [RelayCommand]
        public async Task Edit(Cliente cliente)
        {
            var lotes = new ObservableCollection<string>(Lotes);
            lotes.Remove("Todos");

            var model = new EditViewModel(cliente, lotes);
            var editView = new EditClienteDialog(model);

            if (editView.ShowDialog() ?? false)
                await _clienteService.UpdateCliente(cliente);

            CClientesView.Refresh();
        }

        [RelayCommand]
        public async Task Notes(Cliente cliente)
        {
            var vm = new NotasViewModel(cliente, _notaService);
            await vm.LoadDataAsync();
            var notasView = new NotasDialog(vm);

            notasView.ShowDialog();
        }

        [RelayCommand]
        public async Task Pagos(Cliente cliente)
        {
            var vm = new PagosViewModel(cliente, _pagoService, _clienteService);
            await vm.LoadDataAsync();

            var pagosView = new PagosDialog(vm);
            pagosView.ShowDialog();

            CClientesView.Refresh();
        }

        public ClientesViewModel(ClienteService clienteService, NotaService notaService, PagoService pagoService)
        {
            _clienteService = clienteService;
            _notaService = notaService;
            _pagoService = pagoService;

            CClientesView = CollectionViewSource.GetDefaultView(_clientes);
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var clientes = await _clienteService.GetAll();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {

                    Lotes.Clear();
                    Lotes.Add("Todos");

                    foreach (var cliente in clientes)
                    {
                        string lote = cliente.Lote;
                        if (!Lotes.Contains(lote))
                            Lotes.Add(lote);

                        _clientes.Add(cliente);
                    }

                    OnPropertyChanged(nameof(Lotes));
                });
            }
            catch
            {
                throw;
            }

            return;
        }

        partial void OnLoteSeleccionadoChanged(string value) => CClientesView.Filter = FiltrarClientes;

        partial void OnTextoBusquedaChanged(string value) => CClientesView.Filter = FiltrarClientes;

        private bool FiltrarLotes(object obj)
        {
            if (obj is not Cliente cliente)
                return false;

            if (LoteSeleccionado == "Todos")
                return true;

            return cliente.Lote == LoteSeleccionado;
        }

        private bool FiltrarClientes(object obj)
        {
            if (obj is not Cliente cliente)
                return false;

            bool lote = FiltrarLotes(cliente);

            if (string.IsNullOrWhiteSpace(TextoBusqueda))
                return lote;

            return lote && (cliente.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)
                || cliente.Vehiculo.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)
                || (cliente.Placa != null && cliente.Placa.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
