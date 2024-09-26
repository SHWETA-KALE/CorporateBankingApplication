using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        // GET: Admin
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
                 cd.Id.ToString(), //change column names
                 cd.UserName,
                 cd.Email,
                 cd.CompanyName,
                 cd.ContactInformation,
                 cd.Location,
                 cd.Balance.ToString(),
                 cd.OnBoardingStatus.ToString()
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
    }

}
