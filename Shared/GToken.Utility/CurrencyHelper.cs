
using System;
namespace Platform.Utility
{
    public class CurrencyHelper
    {
        public static decimal Round(decimal amount, string currency)
        {
            decimal param = 0.05m;
            if (currency == "SGD")
            {
                amount = Math.Round(amount / param) * param;
            }
            return Math.Round(amount, 3);
        }
    }


}
