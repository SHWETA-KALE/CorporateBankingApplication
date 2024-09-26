using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using CorporateBankingApplication.Data;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Services;


namespace CorporateBankingApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserDTO userDto)
        {
            var result = _userService.IsLogging(userDto);

            if (result != null)
            {
                var user = _userService.GetUserByUsername(userDto.UserName);
                FormsAuthentication.SetAuthCookie(user.UserName, true);
                //storing the logged in users id in the session
                Session["UserId"] = user.Id;
                if (result == "Admin")
                {
                    return Content("admin here");
                }
                else
                {
                    return RedirectToAction("Index", "Client");
                }
            }
            return View(userDto);

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(ClientDTO clientDTO)
        {
            var uploadedFiles = new List<HttpPostedFileBase>();

            var companyIdProof = Request.Files["uploadedFiles1"];
            var addressProof = Request.Files["uploadedFiles2"];

            //var file1 = Request.Files["uploadedFiles1"]; //this comes from view file name input field names
            //if (file1 != null && file1.ContentLength > 0)
            //{
            //    uploadedFiles.Add(file1);
            //}

            //var file2 = Request.Files["uploadedFiles2"];
            //if (file2 != null && file2.ContentLength > 0)
            //{
            //    uploadedFiles.Add(file2);
            //}

            if (companyIdProof != null && companyIdProof.ContentLength > 0)
            {
                uploadedFiles.Add(companyIdProof);
            }

            if(addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }
            _userService.CreateNewClient(clientDTO, uploadedFiles);
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Client")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}