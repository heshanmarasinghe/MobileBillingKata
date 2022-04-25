using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string BillingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string PackageCode { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}
