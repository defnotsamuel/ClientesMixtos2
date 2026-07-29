using ClientesMixtos.DateUtils;
using ClientesMixtos.Models;
using ClientesMixtos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class AddClienteViewModel : DialogViewModelBase
    {
        private readonly Cliente _cliente = new();
        public Cliente Cliente => _cliente;

        [ObservableProperty]
        private string _nombre = string.Empty;

        [ObservableProperty]
        private string _libro1 = string.Empty;

        [ObservableProperty]
        private string _libro2 = string.Empty;

        [ObservableProperty]
        private string _vehiculo = string.Empty;

        [ObservableProperty]
        private string _placa = string.Empty;

        [ObservableProperty]
        private string _lote = string.Empty;

        [ObservableProperty]
        private string _fechaDeCompra = string.Empty;

        [ObservableProperty]
        private string _fechaDePago = string.Empty;

        [ObservableProperty]
        private string _fechaVence = string.Empty;

        [ObservableProperty]
        private string _fechaMarcada = string.Empty;

        [ObservableProperty]
        private bool _perdido;

        [ObservableProperty]
        private bool _recuperado;

        [ObservableProperty]
        private string _telefono = string.Empty;

        [ObservableProperty]
        private string _ciudad = string.Empty;

        [ObservableProperty]
        private string _refrenda = string.Empty;

        public ObservableCollection<string> Lotes { get; }

        public AddClienteViewModel(ObservableCollection<string> lotes)
        {
            Lotes = lotes;
        }

        [RelayCommand]
        public void Save()
        {
            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Vehiculo) ||
                string.IsNullOrWhiteSpace(Placa) ||
                string.IsNullOrWhiteSpace(Lote) ||
                string.IsNullOrWhiteSpace(FechaDeCompra))
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Utils.ValidarFecha(FechaDeCompra, true, out var fechaCompra))
                return;

            if (!(Utils.ValidarFecha(FechaDePago, false, out var fechaPago) &&
                Utils.ValidarFecha(FechaMarcada, false, out var fechaMarcada) &&
                Utils.ValidarFecha(FechaVence, false, out var fechaVence)))
                return;

            _cliente.Lote = Lote;
            _cliente.Libro1 = Libro1;
            _cliente.Libro2 = Libro2;
            _cliente.Nombre = Nombre;
            _cliente.Vehiculo = Vehiculo;
            _cliente.Placa = Placa;
            _cliente.FechaDeCompra = fechaCompra?.ToString("dd/MM/yyyy") ?? "";
            _cliente.FechaDePago = fechaPago?.ToString("dd/MM/yyyy") ?? "";
            _cliente.FechaMarcada = fechaMarcada?.ToString("dd/MM/yyyy") ?? "";
            _cliente.FechaVence = fechaVence?.ToString("dd/MM/yyyy") ?? "";
            _cliente.Perdido = Perdido;
            _cliente.Recuperado = Recuperado;
            _cliente.Telefono = Telefono;
            _cliente.Ciudad = Ciudad;
            _cliente.Refrenda = Refrenda;

            RequestClose(true);
        }
    }
}
