using ClientesMixtos.Models;
using ClientesMixtos.Services;
using ClientesMixtos.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace ClientesMixtos.ViewModels
{
    public partial class ClientesViewModel : ObservableObject
    {
        private readonly IClienteService _clienteService;
        private readonly INotaService _notaService;
        private readonly IPagoService _pagoService;

        private readonly ObservableCollection<Cliente> _clientes = [];

        public ObservableCollection<string> Lotes { get; } = [];
        public ICollectionView FilteredClientesView { get; set; }

        [ObservableProperty]
        private string _selectedLot = "Todos";

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private Cliente? _selectedCliente;

        public bool HasSelectedClient => SelectedCliente != null;

        partial void OnSelectedClienteChanged(Cliente? value)
        {
            OnPropertyChanged(nameof(HasSelectedClient));
        }

        public ClientesViewModel(IClienteService clienteService, INotaService notaService, IPagoService pagoService)
        {
            _clienteService = clienteService;
            _notaService = notaService;
            _pagoService = pagoService;

            FilteredClientesView = CollectionViewSource.GetDefaultView(_clientes);

            _ = LoadDataAsync();
        }

        private ObservableCollection<string> GetLotesWithoutTodos()
        {
            var lotes = new ObservableCollection<string>(Lotes);
            lotes.Remove("Todos");
            return lotes;
        }

        [RelayCommand]
        public void NewLote(object? target)
        {
            var model = new NewLoteViewModel(GetLotesWithoutTodos());
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
            var model = new AddClienteViewModel(GetLotesWithoutTodos());
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
                return;

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
            FilteredClientesView.Refresh();
        }

        [RelayCommand]
        public async Task MarkAsPaid(Cliente cliente)
        {
            if (MessageBoxResult.Yes != MessageBox.Show($"¿Está seguro de saldar al cliente {cliente.Nombre}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                return;

            cliente.Saldado = true;
            await _clienteService.UpdateCliente(cliente);
            FilteredClientesView.Refresh();
        }

        [RelayCommand]
        public async Task Edit(Cliente cliente)
        {
            var model = new EditViewModel(cliente, GetLotesWithoutTodos());
            var editView = new EditClienteDialog(model);

            if (editView.ShowDialog() ?? false)
                await _clienteService.UpdateCliente(cliente);

            FilteredClientesView.Refresh();
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

            FilteredClientesView.Refresh();
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
        }

        partial void OnSelectedLotChanged(string value) => FilteredClientesView.Filter = FilterClientes;
        partial void OnSearchTextChanged(string value) => FilteredClientesView.Filter = FilterClientes;

        private bool FilterLotes(object obj)
        {
            if (obj is not Cliente cliente)
                return false;

            if (SelectedLot == "Todos")
                return true;

            return cliente.Lote == SelectedLot;
        }

        private bool FilterClientes(object obj)
        {
            if (obj is not Cliente cliente)
                return false;

            bool lote = FilterLotes(cliente);

            if (string.IsNullOrWhiteSpace(SearchText))
                return lote;

            return lote && (cliente.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || cliente.Vehiculo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || (cliente.Placa != null && cliente.Placa.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
