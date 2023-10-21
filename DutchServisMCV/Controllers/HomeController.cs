using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Models;

namespace DutchServisMCV.Controllers
{
    public class HomeController : Controller
    {
        DutchDatabaseEntities database = new DutchDatabaseEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users userInfo)
        {
            var logincheck = database.Users.Where(x => x.Username.Equals(userInfo.Username) && x.Pass.Equals(userInfo.Pass)).FirstOrDefault();
            if(logincheck != null)
            {
                Session["Username"] = userInfo.Username.ToString();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Notification = "Nieprawidłowa nazwa lub hasło";
            }
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Players()
        {
            return View(database.Players);
        }
    }
}