using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace ClientesMixtos.ViewModels
{
    public partial class MarcarDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AceptarCommand))]
        private int _meses = 1;

        public event Action<bool?>? CloseRequested;

        [RelayCommand(CanExecute = nameof(CanAceptar))]
        public void Aceptar()
        {
            CloseRequested?.Invoke(true);
        }

        [RelayCommand]
        public void Cancelar()
        {
            CloseRequested?.Invoke(false);
        }

        private bool CanAceptar()
        {
            return Meses >= 1;
        }
    }
}
