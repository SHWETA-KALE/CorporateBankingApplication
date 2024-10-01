using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Enum;

namespace CorporateBankingApplication.Models
{
    public class Payment
    {
        //public virtual Guid Id { get; set; }

        ////public virtual Client Client { get; set; }

        //public virtual Beneficiary Beneficiary { get; set; }

        //public virtual double Amount { get; set; }

        //public virtual CorporateStatus PaymentStatus { get; set; }
        //public virtual DateTime PaymentRequestDate { get; set; }

        //public virtual DateTime PaymentApprovalDate { get; set; }

        public virtual Guid Id { get; set; }
        public virtual Guid ClientId { get; set; }
        public virtual Beneficiary Beneficiary { get; set; }
        public virtual double Amount { get; set; }
        public virtual DateTime PaymentRequestDate { get; set; }
        public virtual DateTime? PaymentApprovalDate { get; set; }
        public virtual string RazorpayPaymentId { get; set; }
        public virtual CorporateStatus PaymentStatus { get; set; }



    }
}