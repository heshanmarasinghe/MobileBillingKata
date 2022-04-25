using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.Models
{
    public class Package
    {
        public string PackageName { get; set; }
        public int LocalPeak { get; set; }
        public int LocalOffPeak { get; set; }
        public int LongDistancePeak { get; set; }
        public int LongDistanceOffPeak { get; set; }
        public double MonthlyRental { get; set; }
        public double DiscountPercentage { get; set; }
        public bool IsPerMinute { get; set; }
        public bool IsFreeOfCharge { get; set; }
        public bool IsDiscountEligible { get; set; }
        public TimeSpan PeakHoursStartTime { get; set; }
        public TimeSpan PeakHoursEndTime { get; set; }
    }
}
