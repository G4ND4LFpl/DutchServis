using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Models;

namespace DutchServisMCV.Controllers
{
    public class AdminController : Controller
    {
        DutchDatabaseEntities1 database = new DutchDatabaseEntities1();

        private bool IsLoginCorrect(Users user)
        {
            var check = database.Users.Where(x => x.Username.Equals(user.Username) && x.Pass.Equals(user.Pass)).FirstOrDefault();
            return check != null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users userInfo)
        {
            if (IsLoginCorrect(userInfo))
            {
                Session["Username"] = userInfo.Username.ToString();
                return RedirectToAction("Index", "Admin");
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
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
