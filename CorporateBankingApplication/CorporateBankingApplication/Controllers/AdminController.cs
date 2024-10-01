using Azure.Core;
using CorporateBankingApplication.Data;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        // GET: Admin
        public ActionResult AdminDashboard()
        {
            return View();
        }

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
        public ActionResult EditClientDetails(ClientDTO clientDTO, Guid id)
        {
            _adminService.EditClient(clientDTO, id);
            return Json(new { success = true, message = "Client Details Edited successfully." });
        }

        public JsonResult UpdateClientIsActive(Guid id, bool isActive)
        {
            string message;
            bool success = _adminService.ToggleClientActiveStatus(id, isActive, out message);

            return Json(new { success = success, message = message });
        }


        /********************************Verification***********************************************/

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

        [HttpPost]
        public ActionResult UpdateClientOnboardingStatus(Guid id, string status)
        {
            var result = _adminService.UpdateClientOnboardingStatus(id, status);
            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update status" });
            }
        }

        //*************************Salary Disbursement Verification*******************************

        public ActionResult VerifySalaryDisbursement()
        {
            var pendingDisbursements = _adminService.GetPendingSalaryDisbursements();
            return View(pendingDisbursements);
        }

        [HttpPost]
        public ActionResult ApproveDisbursement(Guid salaryDisbursementId)
        {
            bool success = _adminService.ApproveSalaryDisbursement(salaryDisbursementId);

            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Salary disbursement approved successfully."
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to approve salary disbursement. Insufficient balance or invalid request."
                });
            }
        }


        [HttpPost]
        public ActionResult RejectDisbursement(Guid salaryDisbursementId)
        {

            bool success = _adminService.RejectSalaryDisbursement(salaryDisbursementId);

            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Salary disbursement rejected successfully."
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to reject salary disbursement."
                });
            }
        }
        /****************************VERIFY OUTBOUND BENEFICIARIES*******************************/
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

        [HttpPost]
        public ActionResult UpdateOutboundBeneficiaryOnboardingStatus(Guid id, string status)
        {
            var result = _adminService.UpdateOutboundBeneficiaryOnboardingStatus(id, status);
            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update status" });
            }
        }

        /****************************PROFILE*******************************/
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
    }
}