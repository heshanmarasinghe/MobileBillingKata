using Microsoft.AspNetCore.Mvc;
using MobileBillingKata.Models;
using MobileBillingKata.Services;
using MobileBillingKata.ViewModels;
using System;
using System.Collections.Generic;

namespace MobileBillingKata.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;

        public BillingController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        [HttpGet("GetCustomers")]
        public List<Customer> GetCustomerDetails()
        {
            try
            {
                return _billingService.GetCustomerDetails();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetCDR")]
        public List<CallDetailRecord> GetCallDetailRecordDetails()
        {
            try
            {
                return _billingService.GetCallDetailRecordDetails();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GenerateBills")]
        public ActionResult<List<Bill>> GenerateBills()
        {
            try
            {
                return _billingService.GenerateBillDetails();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IActionResult GetBillingDetails()
        {
            CustomerBills customerBills = new CustomerBills
            {
                Customers = _billingService.GetCustomerDetails(),
                Bills = _billingService.GenerateBillDetails()
            };

            List<CallDetailRecord> callDetailRecords = _billingService.GetCallDetailRecordDetails();

            callDetailRecords.ForEach(record =>
            {
                record.CustomerName = customerBills.Customers.Find(x => x.Id == record.CustomerId)?.FullName;
            });
            customerBills.CallDetailRecords = callDetailRecords;

            return View("~/Views/Index.cshtml", customerBills);
        }
    }
}
