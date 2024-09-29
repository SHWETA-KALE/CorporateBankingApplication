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
        public ActionResult AdminDashboard()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
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
                    clientDetailList = sord == "asc" ? clientDetailList.OrderBy(cd => cd.OnBoardingStatus).ToList()
                        : clientDetailList.OrderByDescending(cd => cd.OnBoardingStatus).ToList();
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
                 cd.Id.ToString(),
                 cd.UserName,
                 cd.Email,
                 cd.CompanyName,
                 cd.ContactInformation,
                 cd.Location,
                 cd.Balance.ToString(),
                 cd.OnBoardingStatus.ToString(),
                 cd.IsActive ? "true" : "false"
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

        public ActionResult DeleteClientDetails(Guid id)
        {
            _adminService.RemoveClient(id);
            return Json(new { success = true, message = "Client Deleted successfully." });
        }

        //**********************Verification***********************************************

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

        ////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        public JsonResult UpdateClientIsActive(int id, bool isActive)
        {

            using (var session = NHibernateHelper.CreateSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    var client = session.Get<Client>(id);
                    if (client != null)
                    {
                        // Update the IsActive status
                        client.IsActive = isActive;

                        session.Update(client);
                        transaction.Commit();

                        return Json(new { success = true, message = "IsActive status updated successfully." });
                    }
                    return Json(new { success = false, message = "Client not found." });
                }
            }

        }

    }

}
