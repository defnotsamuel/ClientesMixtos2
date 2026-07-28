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

        public static DateTime ObtenerProximaFechaPago(DateTime fechaCompra)
        {
            fechaCompra = fechaCompra.Date;
            DateTime hoy = DateTime.Today;

            // Si la compra aún no ocurre, el primer pago es un mes después.
            if (fechaCompra > hoy)
                return CrearFechaValida(fechaCompra.AddMonths(1), fechaCompra.Day);

            // Fecha de pago correspondiente al mes actual.
            DateTime fechaPago = CrearFechaValida(hoy, fechaCompra.Day);

            // Si ya pasó este mes, mover al siguiente.
            if (fechaPago < hoy)
                fechaPago = CrearFechaValida(hoy.AddMonths(1), fechaCompra.Day);

            return fechaPago;
        }

        public static DateTime CrearFechaValida(DateTime referencia, int diaDeseado)
        {
            int dia = Math.Min(diaDeseado,
                DateTime.DaysInMonth(referencia.Year, referencia.Month));

            return new DateTime(referencia.Year, referencia.Month, dia);
        }
    }
}
