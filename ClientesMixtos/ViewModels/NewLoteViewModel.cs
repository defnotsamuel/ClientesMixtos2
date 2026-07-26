using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ClientesMixtos.ViewModels
{
    public partial class NewLoteViewModel(ObservableCollection<string> lotes) : ObservableObject
    {

        [ObservableProperty]
        private string _lote = string.Empty;

        private readonly ObservableCollection<string> _lotes = lotes;

        public event Action<bool?>? CloseRequested;

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

            CloseRequested?.Invoke(true);
        }
    }
}
