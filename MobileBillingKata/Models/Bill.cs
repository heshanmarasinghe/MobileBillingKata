using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.Models
{
    public class Bill
    {
        public string CustomerFullName { get; set; }
        public string PhoneNumber { get; set; }
        public string BillingAddress { get; set; }
        public string TotalCallCharges { get; set; }
        public string TotalDiscount { get; set; }
        public string Tax { get; set; }
        public string Rental { get; set; }
        public string BillAmount { get; set; }
    }
}
