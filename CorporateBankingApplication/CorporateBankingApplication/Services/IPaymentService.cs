using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Services
{
    public interface IPaymentService
    {
        void CreatePayment(Guid clientId, Beneficiary beneficiary, double amount, string razorpayPaymentId);
        void UpdatePaymentStatus(string razorpayOrderId, CorporateStatus status);
        void ApprovePayment(Payment payment);



    }
}