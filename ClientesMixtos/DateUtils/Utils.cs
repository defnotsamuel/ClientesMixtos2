using ClientesMixtos.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ClientesMixtos.DateUtils
{
    public static class Utils
    {
        public static DateTime? ParseDate(string date)
        {
            bool success = DateTime.TryParseExact(
                    date,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaPago);

            return success ? fechaPago : null;
        }

        public static void CalculateFechaDePago(Cliente cliente)
        {
            if (cliente.State.FechaDeCompra is null) return;

            DateTime fechaCompra = (DateTime)cliente.State.FechaDeCompra;
            DateTime hoy = DateTime.Now.Date;
            int diaPago = fechaCompra.Day;

            int diasEnMesActual = DateTime.DaysInMonth(hoy.Year, hoy.Month);
            int diaAjustado = Math.Min(diaPago, diasEnMesActual);
            DateTime fechaPago = new(hoy.Year, hoy.Month, diaAjustado);

            if (fechaPago < hoy)
            {
                cliente.State.FechaDePago = fechaPago.AddMonths(1);
                cliente.FechaDePago = ((DateTime)cliente.State.FechaDePago).ToString("dd/MM/yyyy");
                return;
            }

            if (fechaCompra >= hoy)
            {
                int added = fechaCompra.Month - hoy.Month;
                added = added == 0 ? 1 : added + 1;

                DateTime siguienteMes = hoy.AddMonths(added);
                int diasEnSiguienteMes = DateTime.DaysInMonth(siguienteMes.Year, siguienteMes.Month);
                int diaAjustadoSiguiente = Math.Min(diaPago, diasEnSiguienteMes);
                fechaPago = new DateTime(siguienteMes.Year, siguienteMes.Month, diaAjustadoSiguiente);
            }

            cliente.State.FechaDePago = fechaPago;
            cliente.FechaDePago = ((DateTime)cliente.State.FechaDePago).ToString("dd/MM/yyyy");
        }
    }
}
