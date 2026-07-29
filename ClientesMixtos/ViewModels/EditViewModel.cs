using ClientesMixtos.DateUtils;
using ClientesMixtos.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class EditViewModel : DialogViewModelBase
    {
        private readonly Cliente _cliente;

        public ObservableCollection<string> Lotes { get; }

        [ObservableProperty]
        private string _nombre;

        [ObservableProperty]
        private string _libro1;

        [ObservableProperty]
        private string _libro2;

        [ObservableProperty]
        private string _vehiculo;

        [ObservableProperty]
        private string _placa;

        [ObservableProperty]
        private string _lote;

        [ObservableProperty]
        private string _fechaDeCompra;

        [ObservableProperty]
        private string _fechaDePago;

        [ObservableProperty]
        private string _fechaVence;

        [ObservableProperty]
        private string _fechaMarcada;

        [ObservableProperty]
        private bool _ninguno;

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

        public EditViewModel(Cliente cliente, ObservableCollection<string> lotes)
        {
            _cliente = cliente;

            Nombre = cliente.Nombre;
            Vehiculo = cliente.Vehiculo;
            Lote = cliente.Lote;
            Libro1 = cliente.Libro1;
            Libro2 = cliente.Libro2;
            Placa = cliente.Placa;
            FechaDeCompra = cliente.FechaDeCompra;
            FechaDePago = cliente.FechaDePago;
            FechaMarcada = cliente.FechaMarcada;
            Perdido = cliente.Perdido;
            Recuperado = cliente.Recuperado;
            FechaVence = cliente.FechaVence;
            Telefono = cliente.Telefono;
            Ciudad = cliente.Ciudad;
            Refrenda = cliente.Refrenda;

            Ninguno = !cliente.Recuperado && !cliente.Perdido;

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
