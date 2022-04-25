using MobileBillingKata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.ViewModels
{
    public class CustomerBills
    {
        public List<Customer> Customers { get; set; }
        public List<CallDetailRecord> CallDetailRecords { get; set; }
        public List<Bill> Bills { get; set; }
    }
}
