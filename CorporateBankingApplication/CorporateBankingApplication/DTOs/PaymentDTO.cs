using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class PaymentDTO
    {
        public Guid PaymentId { get; set; }
        public string CompanyName { get; set; } //from client

       public string Username { get; set; }
        public  string AccountNumber { get; set; }
        public  Beneficiary Beneficiary {  get; set; }

        //public string BeneficiaryType { get; set; }
        public BeneficiaryType BeneficiaryType { get; set; }

        public string BeneficiaryName { get; set; }
        public double Amount { get; set; }
        public CorporateStatus PaymentStatus { get; set; }
        public  DateTime PaymentRequestDate { get; set; }
        public  string RazorpayPaymentId { get; set; }

        public string ClientName { get; set; }

      

    }
}