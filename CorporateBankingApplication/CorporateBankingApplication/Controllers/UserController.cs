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
using CorporateBankingApplication.Models;
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
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult Login(UserDTO userDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(userDto);
        //    }
        //    var result = _userService.IsLogging(userDto);

        //    if (result != null)
        //    {
        //        var user = _userService.GetUserByUsername(userDto.UserName);
        //        FormsAuthentication.SetAuthCookie(user.UserName, true);
        //        //storing the logged in users id in the session
        //        Session["UserId"] = user.Id;
        //        if (result == "Admin")
        //        {
        //            return RedirectToAction("AdminDashboard", "Admin");
        //        }
        //        else
        //        {
        //            return RedirectToAction("ClientDashboard", "Client");
        //        }
        //    }
        //    return View(userDto);

        //}
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userDto);
            }
            var result = _userService.IsLogging(userDto);
            if (result != null)
            {
                var user = _userService.GetUserByUsername(userDto.UserName);
                if (result == "Client")
                {
                    var client = user as Client;
                    if (client != null && !client.IsActive)
                    {
                        ModelState.AddModelError("", "Account is inactive!!");
                        return View(userDto);
                    }
                }
                FormsAuthentication.SetAuthCookie(user.UserName, true);
                Session["UserId"] = user.Id;
                if (result == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("ClientDashboard", "Client");
                }
            }
            ModelState.AddModelError("", "Username / Password doesn't exist!!");
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
            if (!ModelState.IsValid)
            {
                return View(clientDTO);
            }

            var uploadedFiles = new List<HttpPostedFileBase>();

            var companyIdProof = Request.Files["uploadedFiles1"];
            var addressProof = Request.Files["uploadedFiles2"];

            //for doc validation
            if (companyIdProof == null || companyIdProof.ContentLength == 0)
            {
                ModelState.AddModelError("Document1", "The Company Id Proof field is required.");
            }

            if (addressProof == null || addressProof.ContentLength == 0)
            {
                ModelState.AddModelError("Document2", "The Address Proof field is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(clientDTO);
            }

            if (companyIdProof != null && companyIdProof.ContentLength > 0)
            {
                uploadedFiles.Add(companyIdProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }
            _userService.CreateNewClient(clientDTO, uploadedFiles);
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Client")]
        //[AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult RegisterAdmin()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult RegisterAdmin(Admin admin)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    admin.Password = PasswordHelper.HashPassword(admin.Password);
                    var role = new Role
                    {
                        RoleName = "Admin",
                        User = admin
                    };
                    session.Save(admin);
                    session.Save(role);
                    transaction.Commit();
                    return RedirectToAction("Login");
                }

            }
        }


    }

    
}