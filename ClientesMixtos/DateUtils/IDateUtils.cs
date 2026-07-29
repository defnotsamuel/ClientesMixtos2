using System;

namespace ClientesMixtos.DateUtils
{
    public interface IDateUtils
    {
        DateTime? ParseDate(string date);
        DateTime GetNextPaymentDate(DateTime purchaseDate);
        DateTime CreateValidDate(DateTime reference, int desiredDay);
    }
}
