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
    [RoutePrefix("")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("about-us")]
        public ActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login()
        {
            return View();
        }

       
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userDto);
            }

            // Get CAPTCHA response from the form submission
            var captchaResponse = Request["g-recaptcha-response"];
            if (!ValidateCaptcha(captchaResponse))
            {
                ModelState.AddModelError("RecaptchaError", "Captcha validation failed. Please try again.");
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

        // Add this method to validate reCAPTCHA
        private bool ValidateCaptcha(string captchaResponse)
        {
            var secretKey = "6Ldd_FcqAAAAAGe6HQhVgh-4489Hy2nbeZOBA3qR"; // Your Google reCAPTCHA secret key
            var client = new System.Net.WebClient();
            var result = client.DownloadString(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaResponse}");
            var captchaResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptchaResult>(result);
            // && captchaResult.Score >= 0.3
            return captchaResult.Success;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("register")]
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public ActionResult Register(ClientDTO clientDTO)
        {
            var uploadedFiles = new List<HttpPostedFileBase>();

            var companyIdProof = Request.Files["uploadedFiles1"];
            var addressProof = Request.Files["uploadedFiles2"];

            // Define allowed MIME types
            var allowedFileTypes = new List<string>
{
    "application/pdf",        // PDF files
    "application/msword",     // .doc files
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx files
    "image/jpeg",             // JPEG images
    "image/png",              // PNG images
    "image/gif"               // GIF images
};

            // Define maximum file size in bytes (e.g., 5MB)
            var maxFileSize = 5 * 1024 * 1024; // 5 MB

            // Validation for company ID proof
            if (companyIdProof == null || companyIdProof.ContentLength == 0)
            {
                ModelState.AddModelError("Document1", "The Company Id Proof field is required.");
            }
            else if (!allowedFileTypes.Contains(companyIdProof.ContentType))
            {
                ModelState.AddModelError("Document1", "Invalid file type for Company Id Proof. Allowed types are: PDF, DOC, DOCX, JPEG, PNG, GIF.");
            }
            else if (companyIdProof.ContentLength > maxFileSize)
            {
                ModelState.AddModelError("Document1", "Company Id Proof exceeds the maximum allowed size of 5 MB.");
            }

            // Validation for address proof
            if (addressProof == null || addressProof.ContentLength == 0)
            {
                ModelState.AddModelError("Document2", "The Address Proof field is required.");
            }
            else if (!allowedFileTypes.Contains(addressProof.ContentType))
            {
                ModelState.AddModelError("Document2", "Invalid file type for Address Proof. Allowed types are: PDF, DOC, DOCX, JPEG, PNG, GIF.");
            }
            else if (addressProof.ContentLength > maxFileSize)
            {
                ModelState.AddModelError("Document2", "Address Proof exceeds the maximum allowed size of 5 MB.");
            }

            // If model validation fails, return the view with validation errors
            if (!ModelState.IsValid)
            {
                return View(clientDTO);
            }

            // Add files to the list if they pass the validation
            if (companyIdProof != null && companyIdProof.ContentLength > 0)
            {
                uploadedFiles.Add(companyIdProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }

            // Call service to create new client
            _userService.CreateNewClient(clientDTO, uploadedFiles);

            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Client")]
        [Route("logout")]

        //[AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index");
        }

        //[AllowAnonymous]
        //[HttpGet]

        //public ActionResult RegisterAdmin()
        //{
        //    return View();
        //}


        //[AllowAnonymous]
        //[HttpPost]
        //public ActionResult RegisterAdmin(Admin admin)
        //{
        //    using (var session = NHibernateHelper.CreateSession())
        //    {
        //        using (var transaction = session.BeginTransaction())
        //        {
        //            admin.Password = PasswordHelper.HashPassword(admin.Password);
        //            var role = new Role
        //            {
        //                RoleName = "Admin",
        //                User = admin
        //            };
        //            session.Save(admin);
        //            session.Save(role);
        //            transaction.Commit();
        //            return RedirectToAction("Login");
        //        }

        //    }
        //}


    }

    
}