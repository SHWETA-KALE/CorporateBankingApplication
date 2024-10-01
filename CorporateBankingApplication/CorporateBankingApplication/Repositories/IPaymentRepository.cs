using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Repositories
{
    public interface IPaymentRepository
    {
        void Save(Payment payment);
        Payment GetByRazorpayOrderId(string orderId);
        void Update(Payment payment);
    }
}