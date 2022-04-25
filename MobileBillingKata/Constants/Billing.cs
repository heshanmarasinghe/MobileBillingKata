using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.Constants
{
    public class Billing
    {
        public static readonly string Currency = "LKR";

        public enum HourType
        {
            Peak = 1,
            OffPeak = 2,
        }

        public enum CallType
        {
            Local = 1,
            LongDistance = 2
        }

        public struct Charges
        {
            public const double TaxPercentage = 20;
            public const double MonthlyExceedBonus = 1000;
        }
    }
}
