using ClientesMixtos.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ClientesMixtos.DateUtils
{
    public class Utils : IDateUtils
    {
        public DateTime? ParseDate(string date)
        {
            bool success = DateTime.TryParseExact(
                    date,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaPago);

            return success ? fechaPago : null;
        }

        public DateTime GetNextPaymentDate(DateTime purchaseDate)
        {
            purchaseDate = purchaseDate.Date;
            DateTime hoy = DateTime.Today;

            if (purchaseDate > hoy)
                return CreateValidDate(purchaseDate.AddMonths(1), purchaseDate.Day);

            DateTime fechaPago = CreateValidDate(hoy, purchaseDate.Day);

            if (fechaPago < hoy)
                fechaPago = CreateValidDate(hoy.AddMonths(1), purchaseDate.Day);

            return fechaPago;
        }

        public DateTime CreateValidDate(DateTime reference, int desiredDay)
        {
            int dia = Math.Min(desiredDay,
                DateTime.DaysInMonth(reference.Year, reference.Month));

            return new DateTime(reference.Year, reference.Month, dia);
        }

        public static bool ValidarFecha(string fechaTexto, bool force, out DateTime? result)
        {
            result = null;
            if (string.IsNullOrEmpty(fechaTexto) && !force) return true;

            bool esValida = DateTime.TryParse(fechaTexto, new CultureInfo("es-SV"), out var parsed);

            if (!esValida)
            {
                System.Windows.MessageBox.Show(
                    $"El valor ingresado ({fechaTexto}) no es una fecha válida.",
                    "Error de formato",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning
                );
                return false;
            }

            result = parsed;
            return true;
        }
    }
}
