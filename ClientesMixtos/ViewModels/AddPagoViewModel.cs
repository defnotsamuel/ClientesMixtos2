using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace ClientesMixtos.ViewModels
{
    public partial class AddPagoViewModel : DialogViewModelBase
    {
        [ObservableProperty]
        private DateTime _fechaSeleccionada = DateTime.Today;

        [RelayCommand]
        public void Aceptar()
        {
            RequestClose(true);
        }

        [RelayCommand]
        public void Cancelar()
        {
            RequestClose(false);
        }
    }
}
