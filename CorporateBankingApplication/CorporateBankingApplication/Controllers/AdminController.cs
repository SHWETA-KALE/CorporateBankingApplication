using Azure.Core;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Services;
using iTextSharp.text.pdf;
using OfficeOpenXml;
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
        public ActionResult ViewSalaryDisbursements(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Store filters in ViewBag for the view
            ViewBag.FilterCompanyName = companyName;
            ViewBag.FilterStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.FilterEndDate = endDate?.ToString("yyyy-MM-dd");

            var salaryDisbursements = _adminService.GetAllSalaryDisbursements(companyName, startDate, endDate);
            return View(salaryDisbursements);
        }

        public ActionResult DownloadSalaryDisbursementsPDFReport(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid adminId = (Guid)Session["UserId"];
            var list = _adminService.GetAllSalaryDisbursements(companyName, startDate, endDate); // Get filtered data

            using (var memoryStream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                var table = new PdfPTable(6); // Create a table with 6 columns
                table.AddCell("ID");
                table.AddCell("Client Name");
                table.AddCell("Employee Name");
                table.AddCell("Salary");
                table.AddCell("Disbursement Date");
                table.AddCell("Status");

                // Add data to the table
                foreach (var emp in list)
                {
                    table.AddCell(emp.SalaryDisbursementId.ToString());
                    table.AddCell(emp.ClientName);
                    table.AddCell($"{emp.EmployeeFirstName} {emp.EmployeeLastName}");
                    table.AddCell(emp.Salary.ToString("C")); // Format as currency
                    table.AddCell(emp.DisbursementDate.ToShortDateString());
                    table.AddCell(emp.SalaryStatus.ToString());
                }

                doc.Add(table);
                doc.Close();

                byte[] bytes = memoryStream.ToArray();

                // Add in report table
                _adminService.AddReportInfo(adminId);

                return File(bytes, "application/pdf", "SalaryDisbursement.pdf");
            }
        }

        public ActionResult DownloadSalaryDisbursementsExcelReport(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid adminId = (Guid)Session["UserId"];
            var list = _adminService.GetAllSalaryDisbursements(companyName, startDate, endDate); // Get filtered data

            using (var package = new ExcelPackage())
            {
                // Create a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("Salary Disbursements");

                // Add headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Client Name";
                worksheet.Cells[1, 3].Value = "Employee Name";
                worksheet.Cells[1, 4].Value = "Salary";
                worksheet.Cells[1, 5].Value = "Disbursement Date";
                worksheet.Cells[1, 6].Value = "Status";

                // Add data to the worksheet starting from row 2
                int row = 2;
                foreach (var emp in list)
                {
                    worksheet.Cells[row, 1].Value = emp.SalaryDisbursementId;
                    worksheet.Cells[row, 2].Value = emp.ClientName;
                    worksheet.Cells[row, 3].Value = $"{emp.EmployeeFirstName} {emp.EmployeeLastName}";
                    worksheet.Cells[row, 4].Value = emp.Salary;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00"; // Format as currency
                    worksheet.Cells[row, 5].Value = emp.DisbursementDate.ToShortDateString();
                    worksheet.Cells[row, 6].Value = emp.SalaryStatus.ToString();
                    row++;
                }

                // Auto-fit the columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Convert the Excel package to a byte array
                byte[] bytes = package.GetAsByteArray();

                // Add report info to the database
                _adminService.AddReportInfo(adminId);

                // Return the file for download
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalaryDisbursement.xlsx");
            }
        }


        [Route("paymentreport")]
        public ActionResult ViewPayments(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Store filters in ViewBag for the view
            ViewBag.FilterCompanyName = companyName;
            ViewBag.FilterStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.FilterEndDate = endDate?.ToString("yyyy-MM-dd");

            var payments = _adminService.GetPayments(companyName, startDate, endDate);
            return View(payments);
        }
        public ActionResult DownloadPaymentPDFReport(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid userId = (Guid)Session["UserId"];

            // Pass filters to the GetPayments method
            var list = _adminService.GetPayments(companyName, startDate, endDate);

            // Create a new PDF document
            using (var memoryStream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                // Create a table for better formatting
                var table = new PdfPTable(7); // Create a table with 7 columns
                table.AddCell("ID");
                table.AddCell("Client Name");
                table.AddCell("Account Number");
                table.AddCell("Beneficiary Name");
                table.AddCell("Amount");
                table.AddCell("Payment Request Date");
                table.AddCell("Status");

                // Add data to the table
                foreach (var pay in list)
                {
                    table.AddCell(pay.PaymentId.ToString());
                    table.AddCell(pay.ClientName);
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

                // Add in report table
                _adminService.AddPaymentReportInfo(userId);

                return File(bytes, "application/pdf", "Payment.pdf");
            }
        }

        public ActionResult DownloadPaymentExcelReport(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid userId = (Guid)Session["UserId"];

            // Pass filters to the GetPayments method
            var list = _adminService.GetPayments(companyName, startDate, endDate);

            using (var package = new ExcelPackage())
            {
                // Create a new worksheet in the Excel package
                var worksheet = package.Workbook.Worksheets.Add("Payments");

                // Add headers to the worksheet
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Client Name";
                worksheet.Cells[1, 3].Value = "Account Number";
                worksheet.Cells[1, 4].Value = "Beneficiary Name";
                worksheet.Cells[1, 5].Value = "Amount";
                worksheet.Cells[1, 6].Value = "Payment Request Date";
                worksheet.Cells[1, 7].Value = "Status";

                // Add data to the worksheet starting from row 2
                int row = 2;
                foreach (var pay in list)
                {
                    worksheet.Cells[row, 1].Value = pay.PaymentId;
                    worksheet.Cells[row, 2].Value = pay.ClientName;
                    worksheet.Cells[row, 3].Value = pay.AccountNumber;
                    worksheet.Cells[row, 4].Value = pay.BeneficiaryName;
                    worksheet.Cells[row, 5].Value = pay.Amount;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00"; // Format amount as currency
                    worksheet.Cells[row, 6].Value = pay.PaymentRequestDate.ToShortDateString();
                    worksheet.Cells[row, 7].Value = pay.PaymentStatus.ToString();
                    row++;
                }

                // Auto-fit the columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Convert the Excel package to a byte array
                byte[] bytes = package.GetAsByteArray();

                // Add report info to the database
                _adminService.AddPaymentReportInfo(userId);

                // Return the file for download
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payments.xlsx");
            }
        }

    }
}