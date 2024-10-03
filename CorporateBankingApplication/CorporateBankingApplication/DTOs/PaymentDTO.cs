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
        public  string AccountNumber { get; set; }
        public  Beneficiary Beneficiary {  get; set; }
        public Client Client { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryType { get; set; }
        public double Amount { get; set; }
        public virtual DateTime PaymentRequestDate { get; set; }
        public virtual DateTime PaymentApprovalDate { get; set; }
        public virtual string RazorpayPaymentId { get; set; }

        public CorporateStatus PaymentStatus { get; set; }

    }
}