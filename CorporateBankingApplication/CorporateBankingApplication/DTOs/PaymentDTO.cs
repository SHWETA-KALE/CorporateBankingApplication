using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class PaymentDTO
    {
        [Display(Name = "Id")]
        public Guid PaymentId { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } //from client

       public string Username { get; set; }

        [Display(Name = "Account Number")]
        public  string AccountNumber { get; set; }
        public  Beneficiary Beneficiary {  get; set; }

        //public string BeneficiaryType { get; set; }
        public BeneficiaryType BeneficiaryType { get; set; }

        [Display(Name = "Beneficiary Name")]
        public string BeneficiaryName { get; set; }
        public double Amount { get; set; }

        [Display(Name = "Status")]
        public CorporateStatus PaymentStatus { get; set; }

        [Display(Name = "Requested Date")]
        public  DateTime PaymentRequestDate { get; set; }
        public  string RazorpayPaymentId { get; set; }

        [Display(Name = "Client Name ")]
        public string ClientName { get; set; }

    }
}