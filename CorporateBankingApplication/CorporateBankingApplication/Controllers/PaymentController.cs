using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using CorporateBankingApplication.Services;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Payment = CorporateBankingApplication.Models.Payment;

namespace CorporateBankingApplication.Controllers
{
    public class PaymentController : Controller
    {
        //private const string _key = "rzp_test_n5njYthRxToXNi"; // Your Razorpay API Key
        //private const string _secret = "x1aZcMCBXHwikf5KGvxM8uCS"; // Your Razorpay Secret


        private readonly IClientService _clientService;
        private readonly IPaymentService _paymentService;
        private readonly string key = System.Configuration.ConfigurationManager.AppSettings["RazorpayKey"];
        private readonly string secret = System.Configuration.ConfigurationManager.AppSettings["RazorpaySecret"];

        public PaymentController(IClientService clientService, IPaymentService paymentService)
        {
            _clientService = clientService;
            _paymentService = paymentService;
        }

        [HttpPost]
        public ActionResult InitiatePayment(Guid beneficiaryId, double amount)
        {
            try
            {
                var clientId = (Guid)Session["UserId"];
                var beneficiary = _clientService.GetBeneficiaryById(beneficiaryId);

                if (beneficiary == null)
                {
                    return Json(new { success = false, message = "Beneficiary not found" });
                }

                // Create Razorpay Order
                RazorpayClient client = new RazorpayClient(key, secret);
                Dictionary<string, object> options = new Dictionary<string, object>
                {
                    { "amount", amount * 100 }, // in paise
                    { "currency", "INR" },
                    { "receipt", Guid.NewGuid().ToString() },
                    { "payment_capture", 1 } // Auto-capture
                };

                Order order = client.Order.Create(options);

                // Store the payment details in the database
                _paymentService.CreatePayment(clientId, beneficiary, amount, order["id"].ToString());

                return Json(new
                {
                    success = true,
                    orderId = order["id"].ToString(),
                    razorpayKey = key,
                    amount = amount * 100, // in paise
                    currency = "INR"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult PaymentVerification(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            try
            {
                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_order_id", razorpay_order_id },
                    { "razorpay_payment_id", razorpay_payment_id },
                    { "razorpay_signature", razorpay_signature }
                };

                RazorpayClient client = new RazorpayClient(key, secret);
                Utils.verifyPaymentSignature(attributes);

                // Update payment status in the database
                _paymentService.UpdatePaymentStatus(razorpay_order_id, CorporateStatus.APPROVED);

                return Json(new { success = true, message = "Payment Verified Successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Payment Verification Failed: " + ex.Message });
            }
        }
    }
}
