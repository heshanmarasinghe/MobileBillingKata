using MobileBillingKata.Controllers;
using MobileBillingKata.Models;
using MobileBillingKata.Services;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace MobileBillingKataUnitTests.Controllers
{
    public class BillController
    {
        public Mock<IBillingService> mock = new Mock<IBillingService>();

        [Fact]
        public void GetCustomerDetails()
        {
            // Arrange
            BillingController emp = new BillingController(mock.Object);

            // Act
            List<Customer> customer = new List<Customer>
            {
                new Customer()
                {
                    Id = 1,
                    BillingAddress = "Colombo 01",
                    FullName = "Heshan Marasinghe",
                    PhoneNumber = "071-1288328",
                    PackageCode = "Package A",
                    RegisteredDate = DateTime.Today
                },

                new Customer()
                {
                    Id = 2,
                    BillingAddress = "Colombo 02",
                    FullName = "Gihan De Silva",
                    PhoneNumber = "072-4746382",
                    PackageCode = "Package B",
                    RegisteredDate = DateTime.Today.AddDays(-2)
                },

                new Customer()
                {
                    Id = 3,
                    BillingAddress = "Colombo 01",
                    FullName = "Supul Jayawardane",
                    PhoneNumber = "071-2222222",
                    PackageCode = "Package C",
                    RegisteredDate = DateTime.Today.AddDays(-10)
                }
            };

            // Assert
            mock.Setup(p => p.GetCustomerDetails()).Returns(customer);
            var result = emp.GetCustomerDetails();
            Assert.True(customer.Equals(result));
        }

        [Fact]
        public void GetCallDetails()
        {
            // Arrange
            BillingController emp = new BillingController(mock.Object);

            // Act
            List<CallDetailRecord> callDetailRecords = new List<CallDetailRecord>
            {
                new CallDetailRecord()
                {
                    CustomerId = 1,
                    SubscriberPhoneNumber = "071-1288328",
                    ReceiverPhoneNumber = "071-1288654",
                    StartTime = Convert.ToDateTime("12/31/2016 10:00:00 AM"),
                    CallDuration = 60000
                },

                new CallDetailRecord()
                {
                    CustomerId = 1,
                    SubscriberPhoneNumber = "071-1288328",
                    ReceiverPhoneNumber = "072-1288098",
                    StartTime = Convert.ToDateTime("12/02/2016 03:59:00 PM"),
                    CallDuration = 800
                },

                new CallDetailRecord()
                {
                    CustomerId = 1,
                    SubscriberPhoneNumber = "071-1288328",
                    ReceiverPhoneNumber = "071-1288098",
                    StartTime = Convert.ToDateTime("12/10/2016 06:00:00 PM"),
                    CallDuration = 600
                },

                new CallDetailRecord()
                {
                    CustomerId = 2,
                    SubscriberPhoneNumber = "071-4746382",
                    ReceiverPhoneNumber = "071-1288876",
                    StartTime = Convert.ToDateTime("12/10/2016 01:00:00 AM"),
                    CallDuration = 650
                },

                new CallDetailRecord()
                {
                    CustomerId = 3,
                    SubscriberPhoneNumber = "071-2222222",
                    ReceiverPhoneNumber = "071-1288876",
                    StartTime = Convert.ToDateTime("12/10/2016 09:00:00 AM"),
                    CallDuration = 30
                },

                new CallDetailRecord()
                {
                    CustomerId = 3,
                    SubscriberPhoneNumber = "071-2222222",
                    ReceiverPhoneNumber = "071-1288876",
                    StartTime = Convert.ToDateTime("12/10/2016 01:00:00 AM"),
                    CallDuration = 650
                }
            };

            // Assert
            mock.Setup(p => p.GetCallDetailRecordDetails()).Returns(callDetailRecords);
            var result = emp.GetCallDetailRecordDetails();
            Assert.True(callDetailRecords.Equals(result));
        }
    }
}
