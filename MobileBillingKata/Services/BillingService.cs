using MobileBillingKata.Constants;
using MobileBillingKata.Models;
using MobileBillingKata.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileBillingKata.Services
{
    public class BillingService : IBillingService
    {
        private readonly IBaseRepository<Customer> _customersRepository;
        private readonly IBaseRepository<CallDetailRecord> _callDetailsRepository;
        private readonly IBaseRepository<Package> _packageRepository;

        public BillingService(IBaseRepository<Customer> customersRepository, IBaseRepository<CallDetailRecord> callDetailsRepository,
            IBaseRepository<Package> packageRepository)
        {
            _customersRepository = customersRepository;
            _callDetailsRepository = callDetailsRepository;
            _packageRepository = packageRepository;

            _customersRepository.Add(SeedCustomerDetails());
            _callDetailsRepository.Add(SeedCallRecordDetails());
            _packageRepository.Add(SeedPackageDetails());
        }

        public List<Customer> GetCustomerDetails()
        {
            return _customersRepository.GetAll();
        }

        public List<CallDetailRecord> GetCallDetailRecordDetails()
        {
            return _callDetailsRepository.GetAll();
        }

        public List<Bill> GenerateBillDetails()
        {
            try
            {
                List<Bill> totalBills = new List<Bill>();
                List<Customer> customers = _customersRepository.GetAll();
                List<Package> packages = _packageRepository.GetAll();

                customers.ForEach(customer =>
                {
                    bool predicate(CallDetailRecord x) => x.CustomerId == customer.Id;
                    List<CallDetailRecord> customerCallRecords = _callDetailsRepository.GetAllById(predicate).ToList();
                    Package customerPackage = packages.Where(x => x.PackageName.Contains(customer.PackageCode)).FirstOrDefault();

                    double totalCallCharges = GenerateTotalChargesForCustomer(customerCallRecords, customerPackage);
                    double totalDiscount = CalculateDiscount(customerPackage, totalCallCharges);
                    double tax = (totalCallCharges + customerPackage.MonthlyRental) * (Billing.Charges.TaxPercentage / 100);
                    double totalBillAmount = totalCallCharges + tax + customerPackage.MonthlyRental - totalDiscount;

                    Bill bill = new Bill
                    {
                        CustomerFullName = customer.FullName,
                        PhoneNumber = customer.PhoneNumber,
                        BillingAddress = customer.BillingAddress,
                        TotalCallCharges = string.Format("{0}: {1:N2}", Billing.Currency, totalCallCharges),
                        TotalDiscount = string.Format("{0}: {1:N2}", Billing.Currency, totalDiscount),
                        Tax = string.Format("{0}: {1:N2}", Billing.Currency, tax),
                        Rental = string.Format("{0}: {1:N2}", Billing.Currency, customerPackage.MonthlyRental),
                        BillAmount = string.Format("{0}: {1:N2}", Billing.Currency, totalBillAmount)
                    };

                    totalBills.Add(bill);
                });

                return totalBills;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double GenerateTotalChargesForCustomer(List<CallDetailRecord> callDetailRecords, Package customerPackage)
        {
            double totalCallCharges = 0;

            if (callDetailRecords != null)
            {
                callDetailRecords.ForEach(callDetail =>
                {
                    totalCallCharges += CalculateTotalChargesForCustomer(customerPackage, callDetail);
                });
            }

            return totalCallCharges;
        }

        public double CalculateTotalChargesForCustomer(Package package, CallDetailRecord callDetail)
        {
            double chargePerMinute = TimeSpan.FromSeconds(callDetail.CallDuration).TotalMinutes;
            chargePerMinute = package.IsPerMinute ? Math.Ceiling(chargePerMinute) : chargePerMinute;

            int timeType = ConfigureCallHours(callDetail, package);
            int callType = ConfigureCallType(callDetail);

            return ConfigureTotalCharges(package, callType, timeType, chargePerMinute);
        }

        public double ConfigureTotalCharges(Package package, int callType, int timeType, double chargePerMinute)
        {
            if (callType == (int)Billing.CallType.Local)
            {
                if (package.IsFreeOfCharge)
                {
                    chargePerMinute = chargePerMinute < 1 ? 0 : chargePerMinute - 1;
                }
                if (timeType == (int)Billing.HourType.Peak)
                {
                    return package.LocalPeak * chargePerMinute;
                }
                return package.LocalOffPeak * chargePerMinute;
            }
            else if (callType == (int)Billing.CallType.LongDistance)
            {
                if (timeType == (int)Billing.HourType.Peak)
                {
                    return package.LongDistancePeak * chargePerMinute;
                }
                return package.LongDistanceOffPeak * chargePerMinute;
            }

            return 0;
        }

        public int ConfigureCallType(CallDetailRecord callDetailRecord)
        {
            string subscriberCode = callDetailRecord.SubscriberPhoneNumber.Split("-").FirstOrDefault();
            string receiverCode = callDetailRecord.ReceiverPhoneNumber.Split("-").FirstOrDefault();

            if (subscriberCode == receiverCode)
            {
                return (int)Billing.CallType.Local;
            }
            else
            {
                return (int)Billing.CallType.LongDistance;
            }
        }

        public int ConfigureCallHours(CallDetailRecord callDetailRecord, Package package)
        {
            TimeSpan time = callDetailRecord.StartTime.TimeOfDay;

            if (time >= package.PeakHoursStartTime && time < package.PeakHoursEndTime)
            {
                return (int)Billing.HourType.Peak;
            }

            if (time > package.PeakHoursStartTime || time <= package.PeakHoursEndTime)
            {
                return (int)Billing.HourType.OffPeak;
            }

            return 0;
        }

        public double CalculateDiscount(Package package, double totalCallCharges)
        {
            if (package.IsDiscountEligible && totalCallCharges > Billing.Charges.MonthlyExceedBonus)
            {
                return totalCallCharges * (package.DiscountPercentage / 100);
            }

            return 0;
        }

        public List<Customer> SeedCustomerDetails()
        {
            return new List<Customer>
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
                    FullName = "Andrew Nick",
                    PhoneNumber = "072-4746382",
                    PackageCode = "Package B",
                    RegisteredDate = DateTime.Today.AddDays(-2)
                },

                new Customer()
                {
                    Id = 3,
                    BillingAddress = "Colombo 03",
                    FullName = "Peter James",
                    PhoneNumber = "071-2222222",
                    PackageCode = "Package C",
                    RegisteredDate = DateTime.Today.AddDays(-10)
                }
            };
        }

        public List<CallDetailRecord> SeedCallRecordDetails()
        {
            return new List<CallDetailRecord>
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
                    CallDuration = 600
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
        }

        public List<Package> SeedPackageDetails()
        {
            return new List<Package>
            {
                new Package()
                {
                    PackageName = "Package A",
                    LocalPeak = 3,
                    LocalOffPeak = 2,
                    LongDistancePeak = 5,
                    LongDistanceOffPeak = 4,
                    MonthlyRental = 100,
                    DiscountPercentage = 40,
                    IsPerMinute = true,
                    IsFreeOfCharge = false,
                    IsDiscountEligible = true,
                    PeakHoursStartTime = new TimeSpan(10, 0, 0),
                    PeakHoursEndTime = new TimeSpan(18, 0, 0)
                },

                new Package()
                {
                    PackageName = "Package B",
                    LocalPeak = 4,
                    LocalOffPeak = 3,
                    LongDistancePeak = 6,
                    LongDistanceOffPeak = 5,
                    MonthlyRental = 100,
                    DiscountPercentage = 40,
                    IsPerMinute = false,
                    IsFreeOfCharge = true,
                    IsDiscountEligible = true,
                    PeakHoursStartTime = new TimeSpan(8, 0, 0),
                    PeakHoursEndTime = new TimeSpan(20, 0, 0)
                },

                new Package()
                {
                    PackageName = "Package C",
                    LocalPeak = 2,
                    LocalOffPeak = 1,
                    LongDistancePeak = 3,
                    LongDistanceOffPeak = 2,
                    MonthlyRental = 300,
                    DiscountPercentage = 40,
                    IsPerMinute = true,
                    IsFreeOfCharge = true,
                    IsDiscountEligible = false,
                    PeakHoursStartTime = new TimeSpan(9, 0, 0),
                    PeakHoursEndTime = new TimeSpan(18, 0, 0)
                },

                new Package()
                {
                    PackageName = "Package D",
                    LocalPeak = 3,
                    LocalOffPeak = 2,
                    LongDistancePeak = 5,
                    LongDistanceOffPeak = 4,
                    MonthlyRental = 300,
                    DiscountPercentage = 40,
                    IsPerMinute = false,
                    IsFreeOfCharge = false,
                    IsDiscountEligible = false,
                    PeakHoursStartTime = new TimeSpan(8, 0, 0),
                    PeakHoursEndTime = new TimeSpan(20, 0, 0)
                }
            };
        }
    }
}
