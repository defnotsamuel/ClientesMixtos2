using ClientesMixtos.Models;
using ClientesMixtos.Repositories;
using ClientesMixtos.Services;
using ClientesMixtos.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private readonly PagosClienteRepository _pagosRepo;
        private readonly ClienteRepository _clienteRepo;

        public ObservableCollection<Cliente> _clientes;
        public ObservableCollection<string> Lotes { get; } = [];
        public ICollectionView CClientesView { get; }

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
            var newLoteView = new NewLoteView(model);
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
            var addView = new AddClienteView(model);

            if (addView.ShowDialog() ?? false)
            {
                var cliente = model.Cliente;

                await _clienteService.AddCliente(cliente);

                _clientes.Add(cliente);
                CClientesView.Refresh();
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
            CClientesView.Refresh();
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
            var dialog = new MarcarDialogView(dialogVm);
            if (dialog.ShowDialog() != true) return;

            await _clienteService.MarcarCliente(cliente, dialogVm.Meses);
            CClientesView.Refresh();
        }

        [RelayCommand]
        public async Task Edit(Cliente cliente)
        {
            var lotes = new ObservableCollection<string>(Lotes);
            lotes.Remove("Todos");

            var model = new EditViewModel(cliente, lotes);
            var editView = new EditView(model);

            if (editView.ShowDialog() ?? false)
                await _clienteService.UpdateCliente(cliente);

            CClientesView.Refresh();
        }

        [RelayCommand]
        public async Task Notes(Cliente cliente)
        {
            var vm = new NotasViewModel(cliente, _notaService);
            await vm.LoadDataAsync();
            var notasView = new NotasView(vm);

            notasView.ShowDialog();
        }

        [RelayCommand]
        public async Task Pagos(Cliente cliente)
        {
            var vm = new PagosViewModel(cliente, _pagosRepo, _clienteRepo);
            await vm.LoadDataAsync();
            var pagosView = new PagosView(vm);

            pagosView.ShowDialog();

            await RealoadDataAsync();
        }

        public ClientesViewModel(ClienteService clienteService, NotaService notaService, PagosClienteRepository pagosRepo, ClienteRepository clienteRepo)
        {
            _clienteService = clienteService;
            _notaService = notaService;
            _pagosRepo = pagosRepo;
            _clienteRepo = clienteRepo;

            _clientes = [];
            CClientesView = CollectionViewSource.GetDefaultView(_clientes);
        }

        public async Task RealoadDataAsync()
        {
            try
            {
                var clientes = await _clienteService.GetAll();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _clientes.Clear();
                    foreach (var c in clientes)
                        _clientes.Add(c);

                    Console.WriteLine(CClientesView.IsEmpty);
                    CClientesView.Refresh();
                    OnPropertyChanged(nameof(Lotes));
                });
            }
            catch
            {

                throw;
                // BD desconectada — los datos se cargarán al reconectar
            }

            return;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var clientes = await _clienteService.GetAll();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _clientes.Clear();
                    foreach (var c in clientes) 
                        _clientes.Add(c);

                    Lotes.Clear();
                    Lotes.Add("Todos");
                    foreach (var c in clientes)
                    {
                        string lote = c.Lote;
                        if (!Lotes.Contains(lote))
                            Lotes.Add(lote);
                    }

                    Console.WriteLine(CClientesView.IsEmpty);
                    CClientesView.Refresh();
                    OnPropertyChanged(nameof(Lotes));

                });
            }
            catch
            {

                throw;
                // BD desconectada — los datos se cargarán al reconectar
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
