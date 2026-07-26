using ClientesMixtos.Models;
using ClientesMixtos.Services;
using ClientesMixtos.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class NotasViewModel(Cliente cliente, NotaService notaService) : ObservableObject
    {
        private readonly Cliente _cliente = cliente;
        private readonly NotaService _notaService = notaService;

        public ObservableCollection<Nota> ListaNotas { get; } = [];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        private Nota? _notaSeleccionada;

        public async Task LoadDataAsync()
        {
            var notas = await _notaService.FromClient(_cliente);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ListaNotas.Clear();
                foreach (var n in notas) ListaNotas.Add(n);
            });
        }

        [RelayCommand]
        private async Task Crear()
        {
            var vm = new NotaFormViewModel();
            var ventana = new NotaFormWindow(vm) { Title = "NUEVA NOTA"};

            if (ventana.ShowDialog() == true && vm.NotaResultado is not null)
            {
                await _notaService.Insert(vm.NotaResultado, _cliente);
                ListaNotas.Add(vm.NotaResultado);
            }
        }

        [RelayCommand(CanExecute = nameof(HayNotaSeleccionada))]
        private async Task Editar()
        {
            var vm = new NotaFormViewModel(NotaSeleccionada);
            var ventana = new NotaFormWindow(vm) { Title = "EDITAR NOTA" };

            if (ventana.ShowDialog() == true && vm.NotaResultado is not null)
            {
                await _notaService.Update(vm.NotaResultado);

                var idx = ListaNotas.IndexOf(vm.NotaResultado);
                if (idx >= 0)
                {
                    ListaNotas.RemoveAt(idx);
                    ListaNotas.Insert(idx, vm.NotaResultado);
                }
            }
        }

        [RelayCommand(CanExecute = nameof(HayNotaSeleccionada))]
        private async Task Eliminar()
        {
            var confirmacion = MessageBox.Show(
                $"¿Seguro que quieres eliminar la nota \"{NotaSeleccionada?.Descripcion}\"?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacion == MessageBoxResult.Yes && NotaSeleccionada is not null)
            {
                await _notaService.Delete(NotaSeleccionada);
                ListaNotas.Remove(NotaSeleccionada);
            }
        }

        private bool HayNotaSeleccionada() => NotaSeleccionada != null;
    }
}
