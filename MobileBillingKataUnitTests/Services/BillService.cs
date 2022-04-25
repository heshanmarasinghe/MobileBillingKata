using MobileBillingKata.Constants;
using MobileBillingKata.Models;
using MobileBillingKata.Repositories;
using MobileBillingKata.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MobileBillingKataUnitTests.Services
{
    public class BillService
    { 
        private readonly BillingService _billingService;
        private readonly Mock<IBaseRepository<Customer>> _customersRepository = new Mock<IBaseRepository<Customer>>();
        private readonly Mock<IBaseRepository<CallDetailRecord>> _callDetailsRepository = new Mock<IBaseRepository<CallDetailRecord>>();
        private readonly Mock<IBaseRepository<Package>> _packageRepository = new Mock<IBaseRepository<Package>>();

        public BillService()
        {
            _billingService = new BillingService(_customersRepository.Object, _callDetailsRepository.Object, _packageRepository.Object);
        }

        [Fact]
        public void GetLocalCallType()
        {
            // Arrange
            CallDetailRecord callDetailRecord = new CallDetailRecord
            {
                SubscriberPhoneNumber = "071-1288328",
                ReceiverPhoneNumber = "071-1288654",
            };

            // Act
            int result = _billingService.ConfigureCallType(callDetailRecord);

            // Assert
            Assert.Equal((int)Billing.CallType.Local, result);
        }

        [Fact]
        public void GetLongDistanceCallType()
        {
            // Arrange
            CallDetailRecord callDetailRecord = new CallDetailRecord
            {
                SubscriberPhoneNumber = "072-1288328",
                ReceiverPhoneNumber = "071-1288654",
            };

            // Act            
            int result = _billingService.ConfigureCallType(callDetailRecord);

            // Assert
            Assert.Equal((int)Billing.CallType.LongDistance, result);
        }

        [Fact]
        public void GetPeakTypeForPackageA()
        {
            // Arrange
            CallDetailRecord callDetailRecord = new CallDetailRecord
            {
                StartTime = Convert.ToDateTime("12/31/2016 10:00:00 AM"),
            };
            Package package = new Package
            {
                PackageName = "Package A",
                LocalPeak = 3,
                LocalOffPeak = 2,
                PeakHoursStartTime = new TimeSpan(10, 0, 0),
                PeakHoursEndTime = new TimeSpan(18, 0, 0)
            };

            // Act            
            int result = _billingService.ConfigureCallHours(callDetailRecord, package);

            // Assert
            Assert.Equal((int)Billing.HourType.Peak, result);
        }

        [Fact]
        public void GetOffPeakTypeForPackageA()
        {
            // Arrange
            CallDetailRecord callDetailRecord = new CallDetailRecord
            {
                StartTime = Convert.ToDateTime("12/31/2016 08:00:00 PM"),
            };
            Package package = new Package
            {
                PackageName = "Package A",
                LocalPeak = 3,
                LocalOffPeak = 2,
                PeakHoursStartTime = new TimeSpan(10, 0, 0),
                PeakHoursEndTime = new TimeSpan(18, 0, 0)
            };

            // Act            
            int result = _billingService.ConfigureCallHours(callDetailRecord, package);

            // Assert
            Assert.Equal((int)Billing.HourType.OffPeak, result);
        }

        [Fact]
        public void GetPeakTypeForPackageC()
        {
            // Arrange
            CallDetailRecord callDetailRecord = new CallDetailRecord
            {
                StartTime = Convert.ToDateTime("12/31/2016 11:00:00 AM"),
            };
            Package package = new Package
            {
                PackageName = "Package C",
                LocalPeak = 3,
                LocalOffPeak = 2,
                PeakHoursStartTime = new TimeSpan(9, 0, 0),
                PeakHoursEndTime = new TimeSpan(18, 0, 0)
            };

            // Act            
            int result = _billingService.ConfigureCallHours(callDetailRecord, package);

            // Assert
            Assert.Equal((int)Billing.HourType.Peak, result);
        }

        [Fact]
        public void GetOffPeakTypeForPackageC()
        {
            // Arrange
            CallDetailRecord callDetailRecord = new CallDetailRecord
            {
                StartTime = Convert.ToDateTime("12/31/2016 06:00:00 PM"),
            };
            Package package = new Package
            {
                PackageName = "Package C",
                LocalPeak = 3,
                LocalOffPeak = 2,
                PeakHoursStartTime = new TimeSpan(9, 0, 0),
                PeakHoursEndTime = new TimeSpan(18, 0, 0)
            };

            // Act            
            int result = _billingService.ConfigureCallHours(callDetailRecord, package);

            // Assert
            Assert.Equal((int)Billing.HourType.OffPeak, result);
        }

        [Fact]
        public void GetPeakTypeForPackageD()
        {
            // Arrange
            CallDetailRecord callDetailRecord = new CallDetailRecord
            {
                StartTime = Convert.ToDateTime("12/31/2016 08:00:00 AM"),
            };
            Package package = new Package
            {
                PackageName = "Package D",
                LocalPeak = 3,
                LocalOffPeak = 2,
                PeakHoursStartTime = new TimeSpan(8, 0, 0),
                PeakHoursEndTime = new TimeSpan(20, 0, 0)
            };

            // Act            
            int result = _billingService.ConfigureCallHours(callDetailRecord, package);

            // Assert
            Assert.Equal((int)Billing.HourType.Peak, result);
        }

        [Fact]
        public void GetOffPeakTypeForPackageD()
        {
            // Arrange
            CallDetailRecord callDetailRecord = new CallDetailRecord
            {
                StartTime = Convert.ToDateTime("12/31/2016 10:00:00 PM"),
            };
            Package package = new Package
            {
                PackageName = "Package D",
                LocalPeak = 3,
                LocalOffPeak = 2,
                PeakHoursStartTime = new TimeSpan(8, 0, 0),
                PeakHoursEndTime = new TimeSpan(20, 0, 0)
            };

            // Act            
            int result = _billingService.ConfigureCallHours(callDetailRecord, package);

            // Assert
            Assert.Equal((int)Billing.HourType.OffPeak, result);
        }

        [Fact]
        public void GetDiscountExceedingMonthlyBonus_ForPackageA()
        {
            // Arrange
            double totalCallCharges = 2000;
            Package package = new Package
            {
                PackageName = "Package A",
                IsDiscountEligible = true,
                DiscountPercentage = 40,
            };

            // Act            
            double result = _billingService.CalculateDiscount(package, totalCallCharges);

            // Assert
            Assert.Equal(800, result);
        }

        [Fact]
        public void GetDiscountNotExceedingMonthlyBonus_ForPackageB()
        {
            // Arrange
            double totalCallCharges = 500;
            Package package = new Package
            {
                PackageName = "Package B"
            };

            // Act            
            double result = _billingService.CalculateDiscount(package, totalCallCharges);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetDiscount_ForOtherPackage()
        {
            // Arrange
            double totalCallCharges = 200;
            Package package = new Package
            {
                PackageName = "Package C"
            };

            // Act            
            double result = _billingService.CalculateDiscount(package, totalCallCharges);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetTotalCharges_ForLongDistance_PeakTypeCustomer()
        {
            // Arrange
            double chargePerMinute = 10;
            int timeType = (int)Billing.HourType.Peak;
            int callType = (int)Billing.CallType.LongDistance;
            Package package = new Package
            {
                PackageName = "Package A",
                LongDistancePeak = 5,
                LongDistanceOffPeak = 4,
            };

            // Act            
            double result = _billingService.ConfigureTotalCharges(package, callType, timeType, chargePerMinute);

            // Assert
            Assert.Equal(50, result);
        }

        [Fact]
        public void GetTotalCharges_ForLongDistance_OffPeakTypeCustomer()
        {
            // Arrange
            double chargePerMinute = 10;
            int timeType = (int)Billing.HourType.OffPeak;
            int callType = (int)Billing.CallType.LongDistance;
            Package package = new Package
            {
                PackageName = "Package C",
                LongDistancePeak = 5,
                LongDistanceOffPeak = 4,
            };

            // Act
            double result = _billingService.ConfigureTotalCharges(package, callType, timeType, chargePerMinute);

            // Assert
            Assert.Equal(40, result);
        }

        [Fact]
        public void GetTotalCharges_ForLocal_PeakType_FirstMinuteFree_PackageBCustomer()
        {
            // Arrange
            double chargePerMinute = 10;
            int timeType = (int)Billing.HourType.Peak;
            int callType = (int)Billing.CallType.Local;
            Package package = new Package
            {
                PackageName = "Package B",
                LocalPeak = 4,
                LocalOffPeak = 3,
            };

            // Act            
            double result = _billingService.ConfigureTotalCharges(package, callType, timeType, chargePerMinute);

            // Assert
            Assert.Equal(40, result);
        }

        [Fact]
        public void GetTotalCharges_ForLocal_OffPeakType_FirstMinuteFree_PackageCCustomer()
        {
            // Arrange
            double chargePerMinute = 10;
            int timeType = (int)Billing.HourType.OffPeak;
            int callType = (int)Billing.CallType.Local;
            Package package = new Package
            {
                PackageName = "Package C",
                LocalPeak = 4,
                LocalOffPeak = 3,
            };

            // Act            
            double result = _billingService.ConfigureTotalCharges(package, callType, timeType, chargePerMinute);

            // Assert
            Assert.Equal(30, result);
        }

        [Fact]
        public void GetTotalCharges_ForLocal_PeakType_PackageB_LessThanOneMinuteCustomer()
        {
            // Arrange
            double chargePerMinute = 0.5;
            int timeType = (int)Billing.HourType.Peak;
            int callType = (int)Billing.CallType.Local;
            Package package = new Package
            {
                PackageName = "Package B",
                LocalPeak = 4,
                LocalOffPeak = 3,
            };

            // Act            
            double result = _billingService.ConfigureTotalCharges(package, callType, timeType, chargePerMinute);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void GetTotalCharges_ForLocal_OffPeakType_OtherPackageCustomer()
        {
            // Arrange
            double chargePerMinute = 10;
            int timeType = (int)Billing.HourType.OffPeak;
            int callType = (int)Billing.CallType.Local;
            Package package = new Package
            {
                PackageName = "Package A",
                LocalPeak = 3,
                LocalOffPeak = 2,
            };

            // Act            
            double result = _billingService.ConfigureTotalCharges(package, callType, timeType, chargePerMinute);

            // Assert
            Assert.Equal(20, result);
        }

        [Fact]
        public void GetTotalCallCharges_ForCustomer()
        {
            // Arrange
            List<CallDetailRecord> callDetailRecords = new List<CallDetailRecord>
            {
                new CallDetailRecord()
                {
                    CustomerId = 2,
                    SubscriberPhoneNumber = "071-4746382",
                    ReceiverPhoneNumber = "071-1288876",
                    StartTime = Convert.ToDateTime("12/10/2016 01:00:00 AM"),
                    CallDuration = 600
                }
            };
            Package package = new Package
            {
                PackageName = "Package B",
                LocalPeak = 4,
                LocalOffPeak = 3,
                LongDistancePeak = 6,
                LongDistanceOffPeak = 5,
            };

            // Act            
            double result = _billingService.GenerateTotalChargesForCustomer(callDetailRecords, package);

            // Assert
            Assert.Equal(30, result);
        }

        [Fact]
        public void GetTotalCallCharges_ForCustomer_IfCustomerHasNoCalls()
        {
            // Arrange
            List<CallDetailRecord> callDetailRecords = null;
            Package package = new Package
            {
                PackageName = "Package B",
                LocalPeak = 4,
                LocalOffPeak = 3,
                LongDistancePeak = 6,
                LongDistanceOffPeak = 5,
            };

            // Act            
            double result = _billingService.GenerateTotalChargesForCustomer(callDetailRecords, package);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetTotalCharge_ForPerSecond()
        {
            // Arrange
            CallDetailRecord callDetailRecords = new CallDetailRecord
            {
                CustomerId = 2,
                SubscriberPhoneNumber = "071-4746382",
                ReceiverPhoneNumber = "071-1288876",
                StartTime = Convert.ToDateTime("12/10/2016 01:00:00 AM"),
                CallDuration = 78

            };
            Package package = new Package
            {
                PackageName = "Package B",
                LocalPeak = 4,
                LocalOffPeak = 3,
                LongDistancePeak = 6,
                LongDistanceOffPeak = 5,
                MonthlyRental = 100,
                IsPerMinute = false,
                IsFreeOfCharge = true,
                IsDiscountEligible = true,
                PeakHoursStartTime = new TimeSpan(8, 0, 0),
                PeakHoursEndTime = new TimeSpan(20, 0, 0)
            };

            // Act
            double result = _billingService.CalculateTotalChargesForCustomer(package, callDetailRecords);

            // Assert
            Assert.Equal(0.90, Math.Round(result, 2));
        }

        [Fact]
        public void GetTotalCharge_ForPerMinute()
        {
            CallDetailRecord callDetailRecords = new CallDetailRecord
            {
                CustomerId = 2,
                SubscriberPhoneNumber = "071-4746382",
                ReceiverPhoneNumber = "071-1288876",
                StartTime = Convert.ToDateTime("12/10/2016 01:00:00 AM"),
                CallDuration = 78

            };
            Package package = new Package
            {
                PackageName = "Package A",
                LocalPeak = 3,
                LocalOffPeak = 2,
                LongDistancePeak = 5,
                LongDistanceOffPeak = 4,
                MonthlyRental = 100,
                IsPerMinute = true,
                IsFreeOfCharge = false,
                IsDiscountEligible = true,
                PeakHoursStartTime = new TimeSpan(10, 0, 0),
                PeakHoursEndTime = new TimeSpan(18, 0, 0)
            };

            // Act
            double result = _billingService.CalculateTotalChargesForCustomer(package, callDetailRecords);

            // Assert
            Assert.Equal(4, Math.Round(result, 2));
        }
    }
}
