using ClientesMixtos.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class EditViewModel : ObservableObject
    {

        private readonly Cliente cliente;
        public Cliente Cliente => cliente;
        public ObservableCollection<string> Lotes { get; }

        public event Action<bool?>? CloseRequested;


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
            this.cliente = cliente;

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

            if (!ValidarFecha(FechaDeCompra, true, out var fechaCompra))
                return;

            if (!(ValidarFecha(FechaDePago, false, out var fechaPago) &&
                ValidarFecha(FechaMarcada, false, out var fechaMarcada) &&
                ValidarFecha(FechaVence, false, out var fechaVence)))
                return;

            cliente.Lote = Lote;
            cliente.Libro1 = Libro1;
            cliente.Libro2 = Libro2;

            cliente.Nombre = Nombre;
            cliente.Vehiculo = Vehiculo;
            cliente.Placa = Placa;

            cliente.FechaDeCompra = fechaCompra?.ToString("dd/MM/yyyy") ?? "";
            cliente.FechaDePago = fechaPago?.ToString("dd/MM/yyyy") ?? "";
            cliente.FechaMarcada = fechaMarcada?.ToString("dd/MM/yyyy") ?? "";
            cliente.FechaVence = fechaVence?.ToString("dd/MM/yyyy") ?? "";

            cliente.Perdido = Perdido;
            cliente.Recuperado = Recuperado;

            cliente.Telefono = Telefono;
            cliente.Ciudad = Ciudad;
            cliente.Refrenda = Refrenda;

            CloseRequested?.Invoke(true);
        }

        private static bool ValidarFecha(string fechaTexto, bool force, out DateTime? result)
        {
            result = null;
            if (string.IsNullOrEmpty(fechaTexto) && !force) return true;

            bool esValida = DateTime.TryParse(fechaTexto, new CultureInfo("es-SV"), out var parsed);

            if (!esValida)
            {
                MessageBox.Show(
                    $"El valor ingresado ({fechaTexto}) no es una fecha válida.",
                    "Error de formato",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            result = parsed;
            return true;
        }
    }
}
