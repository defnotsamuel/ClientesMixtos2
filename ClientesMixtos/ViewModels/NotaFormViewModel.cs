using ClientesMixtos.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class NotaFormViewModel: ObservableObject
    {
        private readonly Nota? _notaOriginal;

        [ObservableProperty]
        private string _descripcion = string.Empty;

        [ObservableProperty]
        private DateTime? _fechaCreacion;

        public Action<bool>? CerrarVentana;
        public Nota? NotaResultado { get; private set; }

        public NotaFormViewModel(Nota? notaExistente = null)
        {
            _notaOriginal = notaExistente;

            if (notaExistente != null)
            {
                Descripcion = notaExistente.Descripcion;
                FechaCreacion = notaExistente.State.FechaCreacion;
            }
            else
            {
                FechaCreacion = DateTime.Now;
            }
        }

        [RelayCommand]
        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(Descripcion))
            {
                MessageBox.Show("La descripción no puede estar vacía.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (FechaCreacion == null)
            {
                MessageBox.Show("Selecciona una fecha válida.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var fechaCreacion = FechaCreacion.Value;

            if (_notaOriginal != null)
            {
                _notaOriginal.Descripcion = Descripcion.Trim();
                _notaOriginal.FechaCreacion = fechaCreacion.ToString("dd/MM/yyyy");
                _notaOriginal.State.FechaCreacion = FechaCreacion;
                NotaResultado = _notaOriginal;
            }
            else
            {
                NotaResultado = new Nota
                {
                    Descripcion = Descripcion.Trim(),
                    FechaCreacion = fechaCreacion.ToString("dd/MM/yyyy")
                };

                NotaResultado.State.FechaCreacion = FechaCreacion;
            }

            CerrarVentana?.Invoke(true);
        }

        [RelayCommand]
        private void Cancelar()
        {
            CerrarVentana?.Invoke(false);
        }
    }
}
