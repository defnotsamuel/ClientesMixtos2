using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientesMixtos.ViewModels
{
    public partial class MarcarDialogViewModel : DialogViewModelBase
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AceptarCommand))]
        private int _meses = 1;

        [RelayCommand(CanExecute = nameof(CanAceptar))]
        public void Aceptar()
        {
            RequestClose(true);
        }

        [RelayCommand]
        public void Cancelar()
        {
            RequestClose(false);
        }

        private bool CanAceptar()
        {
            return Meses >= 1;
        }
    }
}
