using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace ClientesMixtos.ViewModels
{
    public partial class AddPagoViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime _fechaSeleccionada = DateTime.Today;

        public event Action<bool?>? CloseRequested;

        [RelayCommand]
        public void Aceptar()
        {
            CloseRequested?.Invoke(true);
        }

        [RelayCommand]
        public void Cancelar()
        {
            CloseRequested?.Invoke(false);
        }
    }
}
