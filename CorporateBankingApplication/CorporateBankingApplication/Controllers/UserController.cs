using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
                if (result == "Admin")
                {
                    return Content("admin here");
                }
                else
                {
                    return Content("client here");
                }
            }
            return View(userDto);

        }

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Register()
        //{
        //    return View();
        //}




        [HttpGet]
        [Authorize(Roles = "Admin,Client")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}