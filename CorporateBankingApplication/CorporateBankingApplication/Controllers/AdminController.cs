using Azure.Core;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
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
    [Authorize(Roles = "Admin")]
    [RoutePrefix("admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;


        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;

        }
        // GET: Admin
        [Route("")]
        public ActionResult AdminDashboard()
        {
            return View();
        }

        [Route("viewclients")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetClientDetails(int page, int rows, string sidx, string sord, bool _search, string searchField, string searchString, string searchOper)
        {

            var clientDetails = _adminService.ViewAllClients();
            var clientDetailList = clientDetails;

            //check if search operation was requested
            if (_search && searchField == "CompanyName" && searchOper == "eq")
            {
                clientDetailList = clientDetails.Where(cd => cd.CompanyName == searchString).ToList();
            }

            //Get total count of records(for pagination)
            int totalCount = clientDetails.Count();
            //Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalCount / rows);

            //for sorting sort acc to username,email,companyname,contactinfo,location,balance,onboarding status
            switch (sidx)
            {
                case "UserName":
                    clientDetailList = sord == "asc" ? clientDetailList.OrderBy(cd => cd.UserName).ToList()
                        : clientDetailList.OrderByDescending(cd => cd.UserName).ToList();
                    break;
                case "Email":
                    clientDetailList = sord == "asc" ? clientDetailList.OrderBy(cd => cd.Email).ToList()
                        : clientDetailList.OrderByDescending(cd => cd.Email).ToList();
                    break;
                case "CompanyName":
                    clientDetailList = sord == "asc" ? clientDetailList.OrderBy(cd => cd.CompanyName).ToList()
                        : clientDetailList.OrderByDescending(cd => cd.CompanyName).ToList();
                    break;
                case "ContactInformation":
                    clientDetailList = sord == "asc" ? clientDetailList.OrderBy(cd => cd.ContactInformation).ToList()
                        : clientDetailList.OrderByDescending(cd => cd.ContactInformation).ToList();
                    break;
                case "Location":
                    clientDetailList = sord == "asc" ? clientDetailList.OrderBy(cd => cd.Location).ToList()
                        : clientDetailList.OrderByDescending(cd => cd.Location).ToList();
                    break;
                case "Balance":
                    clientDetailList = sord == "asc" ? clientDetailList.OrderBy(cd => cd.Balance).ToList()
                        : clientDetailList.OrderByDescending(cd => cd.Balance).ToList();
                    break;
                case "OnBoardingStatus":
                    clientDetailList = sord == "asc" ? clientDetailList.OrderBy(cd => cd.OnboardingStatus).ToList()
                        : clientDetailList.OrderByDescending(cd => cd.OnboardingStatus).ToList();
                    break;
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalCount,
                rows = clientDetailList.Select(cd => new
                {
                    cell = new string[]
                    {
          cd.Id.ToString(), //change column names
          cd.UserName,
          cd.Email,
          cd.CompanyName,
          cd.ContactInformation,
          cd.Location,
          cd.Balance.ToString(),
          cd.AccountNumber,
          cd.IFSC,
          cd.OnboardingStatus.ToString(),
          cd.IsActive.ToString()
                    }
                }).Skip(page - 1 * rows).Take(rows).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateClientIsActive(Guid id, bool isActive)
        {
            string message;
            bool success = _adminService.ToggleClientActiveStatus(id, isActive, out message);

            return Json(new { success = success, message = message });
        }
        public ActionResult EditClientDetails(ClientDTO clientDTO, Guid id)
        {
            _adminService.EditClient(clientDTO, id);
            return Json(new { success = true, message = "Client Details Edited successfully." });
        }

        public ActionResult DeleteClientDetails(Guid id)
        {
            _adminService.RemoveClient(id);
            return Json(new { success = true, message = "Client Deleted successfully." });
        }

        //**********************Verification***********************************************
        [Route("verifyclients")]
        public ActionResult VerifyClientsData()
        {
            return View();
        }
        public ActionResult GetClientsForVerification()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var urlHelper = new UrlHelper(Request.RequestContext); // Create UrlHelper here
            var clientDtos = _adminService.GetClientsForVerification(urlHelper);
            return Json(clientDtos, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult UpdateClientOnboardingStatus(Guid id, string status)
        //{
        //    var result = _adminService.UpdateClientOnboardingStatus(id, status);
        //    if (result)
        //    {
        //        return Json(new { success = true });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Failed to update status" });
        //    }
        //}

        //public ActionResult UpdateClientOnboardingStatus(List<Guid> id, string status)
        //{
        //    bool allSucceeded = true; // Flag to track if all updates succeeded
        //    foreach (var clientid in id)
        //    {
        //        var result = _adminService.UpdateClientOnboardingStatus(clientid, status);
        //        if (!result)
        //        {
        //            allSucceeded = false; // Set to false if any client update fails
        //        }
        //    }

        //    if (allSucceeded)
        //    {
        //        return Json(new { success = true });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "One or more clients failed to update status" });
        //    }
        //}

        public ActionResult UpdateClientOnboardingStatus(List<Guid> id, string status, string rejectionReason = "")
        {
            bool allSucceeded = true; // Flag to track if all updates succeeded
            foreach (var clientid in id)
            {
                var result = _adminService.UpdateClientOnboardingStatus(clientid, status, rejectionReason);
                if (!result)
                {
                    allSucceeded = false; // Set to false if any client update fails
                }
            }

            if (allSucceeded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "One or more clients failed to update status" });
            }
        }

        //*************************Salary Disbursement Verification*******************************

        [Route("verifydisbursements")]
        public ActionResult VerifySalaryDisbursement()
        {
            var pendingDisbursements = _adminService.GetPendingSalaryDisbursements();
            return View(pendingDisbursements);
        }

        [HttpPost]
        public ActionResult ApproveDisbursements(List<Guid> disbursementIds)
        {
            if (disbursementIds == null || !disbursementIds.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No salary disbursement selected for approval."
                });
            }
            bool success = true;
            foreach (var id in disbursementIds)
            {
                bool approved = _adminService.ApproveSalaryDisbursement(id, true);
                if (!approved)
                {
                    success = false;
                    break;
                }
            }
            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Salary disbursements approved successfully."
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to approve salary disbursements."
                });
            }
        }


        [HttpPost]
        public ActionResult RejectDisbursements(List<Guid> disbursementIds)
        {

            if (disbursementIds == null || !disbursementIds.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No salary disbursement selected for rejection."
                });
            }

            bool success = true;
            foreach (var id in disbursementIds)
            {
                bool rejected = _adminService.RejectSalaryDisbursement(id, true);
                if (!rejected)
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Salary disbursements rejected successfully."
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to reject salary disbursements."
                });
            }
        }

        /**************************VERIFY OUTBOUND BENEFICIARIES*****************************/

        [Route("verifybeneficiaries")]
        public ActionResult VerifyOutboundBeneficiaryData()
        {
            return View();
        }
        public ActionResult GetOutboundBeneficiaryForVerification()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var urlHelper = new UrlHelper(Request.RequestContext); // Create UrlHelper here
            var beneficiaryDtos = _adminService.GetBeneficiariesForVerification(urlHelper);
            return Json(beneficiaryDtos, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult UpdateOutboundBeneficiaryOnboardingStatus(List<Guid> id, string status)
        //{
        //    bool allSucceeded = true; // Flag to track if all updates succeeded
        //    foreach (var beneficiaryid in id)
        //    {
        //        var result = _adminService.UpdateOutboundBeneficiaryOnboardingStatus(beneficiaryid, status);
        //        if (!result)
        //        {
        //            allSucceeded = false; // Set to false if any client update fails
        //        }
        //    }

        //    if (allSucceeded)
        //    {
        //        return Json(new { success = true });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "One or more beneficiaries failed to update status" });
        //    }
        //}

        public ActionResult UpdateOutboundBeneficiaryOnboardingStatus(List<Guid> id, string status, string rejectionReason = "")
        {
            bool allSucceeded = true; // Flag to track if all updates succeeded
            foreach (var beneficiaryid in id)
            {
                var result = _adminService.UpdateOutboundBeneficiaryOnboardingStatus(beneficiaryid, status, rejectionReason);
                if (!result)
                {
                    allSucceeded = false; // Set to false if any client update fails
                }
            }

            if (allSucceeded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "One or more beneficiaries failed to update status" });
            }
        }
        /**************************PROFILE*****************************/
        [Route("profile")]
        public ActionResult ViewAdminProfile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid adminId = (Guid)Session["UserId"];
            var admin = _adminService.GetAdminById(adminId);
            return View(admin);
        }

        //PAYMENT VERIFICATION

        [Route("verifypayments")]
        public ActionResult VerifyPayments()
        {
            var pendingPayments = _adminService.GetPendingPaymentsByStatus(CorporateStatus.PENDING);
            return View(pendingPayments);
        }
        [HttpPost]
        public JsonResult ApprovePayments(List<Guid> disbursementIds)
        {
            if (disbursementIds == null || !disbursementIds.Any())
            {
                return Json(new { success = false, message = "No payments selected for approval." });
            }

            try
            {
                _adminService.UpdatePaymentStatuses(disbursementIds, CorporateStatus.APPROVED);
                return Json(new { success = true, message = "Payments approved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error while approving payments: {ex.Message}" });
            }
        }

        // Reject payments
        [HttpPost]
        public JsonResult RejectPayments(List<Guid> disbursementIds)
        {
            if (disbursementIds == null || !disbursementIds.Any())
            {
                return Json(new { success = false, message = "No payments selected for rejection." });
            }

            try
            {
                _adminService.UpdatePaymentStatuses(disbursementIds, CorporateStatus.REJECTED);
                return Json(new { success = true, message = "Payments rejected successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error while rejecting payments: {ex.Message}" });
            }
        }

        //******************* Analytics **************

        //public ActionResult Analytics()
        //{
        //    return View();
        //}

        //[Route("getanalytics")]
        //public ActionResult GetAnalyticsData()
        //{
        //    var analyticsData = _adminService.GetAnalyticsData();
        //    return Json(analyticsData, JsonRequestBehavior.AllowGet);
        //}

        /*********************************REPORTS **************************************/

        [Route("report")]
        public ActionResult ReportView()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        [Route("salaryreport")]
        public ActionResult ViewSalaryDisbursements()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var salaryDisbursements = _adminService.GetAllSalaryDisbursements();
            return View(salaryDisbursements);
        }

        public ActionResult DownloadSalaryDisbursementsPDFReport()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid adminId = (Guid)Session["UserId"];
            var list = _adminService.GetAllSalaryDisbursements();
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
                _adminService.AddReportInfo(adminId);

                return File(bytes, "application/pdf", "SalaryDisbursement.pdf");
            }
        }

         [Route("paymentreport")]
        public ActionResult ViewPayments()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var payments = _adminService.GetPayments();
            return View(payments);
        }

        public ActionResult DownloadPaymentPDFReport()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid userId = (Guid)Session["UserId"];
            var list = _adminService.GetPayments();

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
                _adminService.AddPaymentReportInfo(userId);

                return File(bytes, "application/pdf", "Payment.pdf");
            }
        }

    }
}