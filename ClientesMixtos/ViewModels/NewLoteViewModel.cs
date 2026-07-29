using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class NewLoteViewModel : DialogViewModelBase
    {
        [ObservableProperty]
        private string _lote = string.Empty;

        private readonly ObservableCollection<string> _lotes;

        public NewLoteViewModel(ObservableCollection<string> lotes)
        {
            _lotes = lotes;
        }

        [RelayCommand]
        public void Guardar()
        {
            if (string.IsNullOrWhiteSpace(Lote))
            {
                MessageBox.Show("El número de lote no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_lotes.Contains(Lote))
            {
                MessageBox.Show($"El lote '{Lote}' ya existe.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Lote {Lote} creado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            RequestClose(true);
        }
    }
}
