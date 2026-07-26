using ClientesMixtos.Models;
using ClientesMixtos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace ClientesMixtos.ViewModels
{
    public partial class AddClienteViewModel(ObservableCollection<string> lotes) : ObservableObject
    {
        private readonly Cliente cliente = new();
        public Cliente Cliente => cliente;

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

        public ObservableCollection<string> Lotes { get; } = lotes;

        public event Action<bool?>? CloseRequested;

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
