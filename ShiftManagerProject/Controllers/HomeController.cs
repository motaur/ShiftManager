using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ShiftManagerProject.Models;
using ShiftManagerProject.DAL;
using System.Web.SessionState;

namespace ShiftManagerProject.Controllers
{
    public class HomeController : Controller
    {
        private EmployeeRepository EmployRes = new EmployeeRepository();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Employees userr)
        {
            var userFromDb = EmployRes.IsValid(userr.Email, userr.Pass);
            if (userFromDb != null)
            {
                HttpContext context = System.Web.HttpContext.Current;
                context.Session["UserSess"] = userFromDb.FirstName;
                Session["admin"] = userFromDb.Admin;

                FormsAuthentication.SetAuthCookie(userFromDb.FirstName, false);
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string userData = serializer.Serialize(userr);
                var authTicket = new FormsAuthenticationTicket(1, userr.Email, DateTime.Now, DateTime.Now.AddMinutes(30), false, userData);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                HttpContext.Response.Cookies.Add(authCookie);

                var authCookies = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authTicket != null && !authTicket.Expired)
                {
                    var roles = authTicket.UserData.Split(',');
                    HttpContext.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                }
                if (userFromDb.Admin==true)
                {
                    return RedirectToAction("ListOfShifts", "FinalShifts");
                }
                return RedirectToAction("Index", "ShiftPrefs", new { id = userFromDb.ID });
            }
            else
            {
                ModelState.AddModelError("", "Login details are incorrect.");
                return View();
            }

        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}