using CorporateBankingApplication.Data;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Services;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApplication.Controllers
{
    [Authorize(Roles = "Admin,Client")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IClientService _clientService;

        public ReportController(IReportService reportService, IClientService clientService)
        {
            _reportService = reportService;
            _clientService = clientService;

        }
        
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        public ActionResult ViewSalaryDisbursements()
        {


            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid userId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(userId);
            ViewBag.Client = client;
            if (User.IsInRole("Admin"))
            {
                string role = "Admin";
                var salaryDisbursements = _reportService.GetSalaryDisbursements(role, userId);
                return View(salaryDisbursements);
            }
            else
            {
                string role = "Client";
                var salaryDisbursements = _reportService.GetSalaryDisbursements(role, userId);
                return View(salaryDisbursements);
            }

        }

        public ActionResult DownloadSalaryDisbursementsPDFReport()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid userId = (Guid)Session["UserId"];
            string role;
            List<EmployeeSalaryDisbursementDTO> list = null; 
            if (User.IsInRole("Admin"))
            {
                role = "Admin";
                var salaryDisbursements = _reportService.GetSalaryDisbursements(role,userId);
                list = salaryDisbursements;
            }
            else
            {
                role = "Client";
                var salaryDisbursements = _reportService.GetSalaryDisbursements(role,userId);
                list = salaryDisbursements;
            }
            // Create a new PDF document
            using (var memoryStream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                // Create a table for better formatting
                var table = new PdfPTable(6); // Create a table with 5 columns
                table.AddCell("ID");
                table.AddCell("Client Company Name");
                table.AddCell("Employee Name");
                table.AddCell("Salary");
                table.AddCell("Disbursement Date");
                table.AddCell("Status");

                // Add data to the table
                foreach (var emp in list)
                {
                    table.AddCell(emp.SalaryDisbursementId.ToString());
                    table.AddCell(emp.CompanyName);
                    table.AddCell($"{emp.EmployeeFirstName} {emp.EmployeeLastName}");
                    table.AddCell(emp.Salary.ToString("C")); // Format as currency
                    table.AddCell(emp.DisbursementDate.ToShortDateString());
                    table.AddCell(emp.SalaryStatus.ToString());
                }

                // Add the table to the document
                doc.Add(table);
                doc.Close(); // Closing the document finalizes it

                // Prepare the byte array to return
                byte[] bytes = memoryStream.ToArray();

                //add in report table
                _reportService.AddReportInfo(role,userId);

                return File(bytes, "application/pdf", "SalaryDisbursement.pdf");
            }
        }

        public ActionResult ViewPayments()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid userId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(userId);
            ViewBag.Client = client;
            if (User.IsInRole("Admin"))
            {
                string role = "Admin";
                var payments = _reportService.GetPayments(role, userId);
                return View(payments);
            }
            else
            {
                string role = "Client";
                var payments = _reportService.GetPayments(role, userId);
                return View(payments);
            }

        }

        public ActionResult DownloadPaymentPDFReport()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid userId = (Guid)Session["UserId"];
            string role;
            List<PaymentDTO> list = null;
            if (User.IsInRole("Admin"))
            {
                role = "Admin";
                var payments = _reportService.GetPayments(role, userId);
                list = payments;
            }
            else
            {
                role = "Client";
                var payments = _reportService.GetPayments(role, userId);
                list = payments;
            }
            // Create a new PDF document
            using (var memoryStream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                // Create a table for better formatting
                var table = new PdfPTable(7); // Create a table with 5 columns
                table.AddCell("ID");
                table.AddCell("Client Company Name");
                table.AddCell("Account Number");
                table.AddCell("Beneficiary Name");
                table.AddCell("Amount");
                table.AddCell("Payment Request Date");
                table.AddCell("Status");

                // Add data to the table
                foreach (var pay in list)
                {
                    table.AddCell(pay.PaymentId.ToString());
                    table.AddCell(pay.CompanyName);
                    table.AddCell(pay.AccountNumber);
                    table.AddCell(pay.BeneficiaryName); // Format as currency
                    table.AddCell(pay.Amount.ToString()); // Format as currency
                    table.AddCell(pay.PaymentRequestDate.ToShortDateString());
                    table.AddCell(pay.PaymentStatus.ToString());
                }

                // Add the table to the document
                doc.Add(table);
                doc.Close(); // Closing the document finalizes it

                // Prepare the byte array to return
                byte[] bytes = memoryStream.ToArray();

                //add in report table
                _reportService.AddPaymentReportInfo(role, userId);

                return File(bytes, "application/pdf", "Payment.pdf");
            }
        }
    }
}