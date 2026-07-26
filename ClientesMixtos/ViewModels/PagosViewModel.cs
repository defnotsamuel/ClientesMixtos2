using ClientesMixtos.Models;
using ClientesMixtos.Repositories;
using ClientesMixtos.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ClientesMixtos.ViewModels
{
    public partial class PagosViewModel(Cliente cliente, PagosClienteRepository pagosRepo, ClienteRepository clienteRepo) : ObservableObject
    {
        private readonly Cliente _cliente = cliente;
        private readonly PagosClienteRepository _pagosRepo = pagosRepo;
        private readonly ClienteRepository _clienteRepo = clienteRepo;

        public ObservableCollection<Pago> ListaPagos { get; } = [];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Pago? _pagoSeleccionado;

        [RelayCommand(CanExecute = nameof(HayPagoSeleccionado))]
        private async Task Eliminar()
        {
            if (PagoSeleccionado is null) return;

            var confirmacion = MessageBox.Show(
                $"¿Seguro que quieres eliminar el pago del {PagoSeleccionado.FechaPagada:dd/MM/yyyy}?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;

            var pagosCliente = await _pagosRepo.GetByClienteId(_cliente.ClienteId);
            if (pagosCliente is null) return;

            pagosCliente.Pagos.RemoveAll(p => p.Id == PagoSeleccionado.Id);
            await _pagosRepo.Update(pagosCliente);
            ListaPagos.Remove(PagoSeleccionado);

            var hoy = DateTime.Now;
            int dia = _cliente.State.FechaDeCompra?.Day ?? 1;

            if (pagosCliente.Pagos.Count > 0)
            {
                var ultimaFecha = pagosCliente.Pagos.Max(p => p.FechaPagada);
                var siguienteMes = ultimaFecha.AddMonths(1);
                _cliente.State.FechaDePago = siguienteMes;
                _cliente.FechaDePago = siguienteMes.ToString("dd/MM/yyyy");
            }
            else
            {
                int diasEnMes = DateTime.DaysInMonth(hoy.Year, 7);
                int diaAjustado = Math.Min(dia, diasEnMes);
                var fechaJulio = new DateTime(hoy.Year, 7, diaAjustado);
                _cliente.State.FechaDePago = fechaJulio;
                _cliente.FechaDePago = fechaJulio.ToString("dd/MM/yyyy");
            }

            await _clienteRepo.UpdateFechaPago(_cliente);
        }

        private bool HayPagoSeleccionado() => PagoSeleccionado != null;

        [RelayCommand]
        private async Task AgregarPago()
        {
            var vm = new AddPagoViewModel();
            var dialog = new AddPagoView(vm);
            if (dialog.ShowDialog() != true) return;

            var pagosCliente = await _pagosRepo.GetByClienteId(_cliente.ClienteId);
            if (pagosCliente is null) return;

            var pago = new Pago
            {
                Id = Guid.NewGuid(),
                FechaPagada = vm.FechaSeleccionada,
                FechaMarcado = DateTime.Now
            };

            pagosCliente.Pagos.Add(pago);
            await _pagosRepo.Update(pagosCliente);
            ListaPagos.Add(pago);
        }

        [RelayCommand]
        private async Task RellenarPagos()
        {
            var pagosCliente = await _pagosRepo.GetByClienteId(_cliente.ClienteId);
            if (pagosCliente is null) return;

            var hoy = DateTime.Now;
            int dia = _cliente.State.FechaDeCompra?.Day ?? 1;
            bool cambio = false;

            DateTime fechaHasta;

            if (pagosCliente.Pagos.Count > 0)
            {
                fechaHasta = pagosCliente.Pagos.Max(p => p.FechaPagada);
            }
            else
            {
                fechaHasta = new DateTime(hoy.Year, hoy.Month,
                    Math.Min(dia, DateTime.DaysInMonth(hoy.Year, hoy.Month)));
            }

            var fechaFin = new DateTime(hoy.Year, 1, Math.Min(dia, DateTime.DaysInMonth(hoy.Year, 1)));
            var actual = fechaHasta;

            while (actual >= fechaFin)
            {
                bool yaExiste = pagosCliente.Pagos.Any(p =>
                    p.FechaPagada.Year == actual.Year && p.FechaPagada.Month == actual.Month);

                if (!yaExiste)
                {
                    int diasEnMes = DateTime.DaysInMonth(actual.Year, actual.Month);
                    int diaAjustado = Math.Min(dia, diasEnMes);
                    pagosCliente.Pagos.Add(new Pago
                    {
                        FechaPagada = new DateTime(actual.Year, actual.Month, diaAjustado),
                        FechaMarcado = DateTime.MinValue
                    });
                    cambio = true;
                }
                actual = actual.AddMonths(-1);
            }

            if (cambio)
            {
                pagosCliente.Pagos.Sort((a, b) => a.FechaPagada.CompareTo(b.FechaPagada));
                await _pagosRepo.Update(pagosCliente);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ListaPagos.Clear();
                    foreach (var p in pagosCliente.Pagos.OrderBy(p => p.FechaPagada))
                        ListaPagos.Add(p);
                });
            }
        }

        public async Task LoadDataAsync()
        {
            var pagosCliente = await _pagosRepo.GetByClienteId(_cliente.ClienteId);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ListaPagos.Clear();
                if (pagosCliente is not null)
                {
                    var ordenados = pagosCliente.Pagos.OrderBy(p => p.FechaPagada).ToList();
                    foreach (var p in ordenados)
                        ListaPagos.Add(p);
                }
            });
        }
    }
}
