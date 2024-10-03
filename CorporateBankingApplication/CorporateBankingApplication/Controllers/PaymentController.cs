
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using CorporateBankingApplication.Services;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CorporateBankingApplication.Controllers
{
    public class PaymentController : Controller
    {
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
            { "payment_capture", 1 } // Auto-capture
        };

                var order = client.Order.Create(options);

                // Store the beneficiaryId and amount in session or database to reference later
                Session["BeneficiaryId"] = beneficiaryId;
                Session["Amount"] = amount;

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
                if (string.IsNullOrEmpty(razorpay_payment_id) ||
                    string.IsNullOrEmpty(razorpay_order_id) ||
                    string.IsNullOrEmpty(razorpay_signature))
                {
                    return Json(new { success = false, message = "Invalid payment details received." });
                }

                var attributes = new Dictionary<string, string>
        {
            { "razorpay_order_id", razorpay_order_id },
            { "razorpay_payment_id", razorpay_payment_id },
            { "razorpay_signature", razorpay_signature }
        };

                RazorpayClient client = new RazorpayClient(key, secret);

                // Verify payment signature
                Utils.verifyPaymentSignature(attributes); // This will throw an exception if verification fails

                // Fetch the payment status from Razorpay
                var payment = client.Payment.Fetch(razorpay_payment_id);
                var paymentStatus = payment["status"].ToString();

                // Only create the payment record if the payment is captured
                if (paymentStatus == "captured")
                {
                    var clientId = (Guid)Session["UserId"];
                    var beneficiaryId = (Guid)Session["BeneficiaryId"];
                    double amount = (double)Session["Amount"];

                    var beneficiary = _clientService.GetBeneficiaryById(beneficiaryId);
                    if (beneficiary == null)
                    {
                        return Json(new { success = false, message = "Beneficiary not found." });
                    }

                    // Create payment record
                    _paymentService.CreatePayment(clientId, beneficiary, amount, razorpay_payment_id);

                    return Json(new { success = true, message = "Payment Verified Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Payment was not successful" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Payment Verification Failed: " + ex.Message });
            }
        }
    }
}
