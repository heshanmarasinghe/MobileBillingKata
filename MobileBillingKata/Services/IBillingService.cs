using MobileBillingKata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.Services
{
    public interface IBillingService
    {
        List<Customer> GetCustomerDetails();
        List<CallDetailRecord> GetCallDetailRecordDetails();
        List<Bill> GenerateBillDetails();
    }
}
