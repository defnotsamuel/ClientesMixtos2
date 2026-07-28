using ClientesMixtos.Models;
using ClientesMixtos.Repos;
using ClientesMixtos.Services;
using ClientesMixtos.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class PagosViewModel(Cliente cliente, PagoService pagoService, ClienteService clienteService) : ObservableObject
    {
        private readonly Cliente _cliente = cliente;
        private readonly PagoService pagoService = pagoService;
        private readonly ClienteService clienteService = clienteService;

        public ObservableCollection<Pago> ListaPagos { get; } = [];


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Pago? _pagoSeleccionado;

        [RelayCommand(CanExecute = nameof(HayPagoSeleccionado))]
        private async Task Eliminar()
        {
            if (PagoSeleccionado is null)
                return;

            var confirmacion = MessageBox.Show(
                $"¿Seguro que quieres eliminar el pago del {PagoSeleccionado.FechaPagada:dd/MM/yyyy}?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes)
                return;

            await pagoService.EliminarPago(PagoSeleccionado.Id);

            ListaPagos.Remove(PagoSeleccionado);

            if (ListaPagos.Any())
            {
                DateTime siguientePago = ListaPagos
                    .Max(p => p.FechaPagada)
                    .AddMonths(1);

                DateTime? fechaMarcada = ListaPagos
                    .Max(p => p.FechaMarcado);

                _cliente.State.FechaDePago = siguientePago;
                _cliente.State.FechaMarcada = fechaMarcada;

                _cliente.FechaDePago = siguientePago.ToString("dd/MM/yyyy");
            }
            else
            {
                DateTime fechaCompra = _cliente.State.FechaDeCompra!.Value;

                _cliente.State.FechaDePago = null;
                _cliente.State.FechaMarcada = null;

                await clienteService.CalculateFechaDePago(_cliente);
            }

            _cliente.FechaMarcada = _cliente.State.FechaMarcada.HasValue ?
                _cliente.State.FechaMarcada.Value.ToString("dd/MM/yyyy") : "";

            await clienteService.UpdateCliente(_cliente);
        }


        [RelayCommand]
        private async Task AgregarPago()
        {
            var vm = new AddPagoViewModel();
            var dialog = new AddPagoDialog(vm);

            if (dialog.ShowDialog() != true)
                return;

            var pago = await pagoService.RegistrarPago(
                _cliente.ClienteId,
                vm.FechaSeleccionada, DateTime.Now.Date);

            if (pago is not null)
                ListaPagos.Add(pago);
        }

        public async Task LoadDataAsync()
        {
            var pagosCliente = await pagoService.GetHistorial(_cliente.ClienteId);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ListaPagos.Clear();
                if (pagosCliente is not null)
                {
                    var ordenados = pagosCliente.OrderBy(p => p.FechaPagada).ToList();
                    foreach (var p in ordenados)
                        ListaPagos.Add(p);
                }
            });
        }


        private bool HayPagoSeleccionado() => PagoSeleccionado != null;
    }
}
