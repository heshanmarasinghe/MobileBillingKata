using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.Models
{
    public class CallDetailRecord
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string SubscriberPhoneNumber { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public DateTime StartTime { get; set; }
        public int CallDuration { get; set; }
    }
}
