using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Models;
using DutchServisMCV.Logic;

namespace DutchServisMCV.Controllers
{
    public class AdminController : Controller
    {
        DutchDatabaseEntities database = new DutchDatabaseEntities();
        PasswordHasher hasher = new PasswordHasher();

        private bool IsLoginCorrect(Users userInfo)
        {
            var user = database.Users.Find(userInfo.Username);
            if (user == null) return false;

            return hasher.Verify(userInfo.Pass, user.Pass);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users userInfo)
        {
            // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
            // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
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
        public ActionResult Index(bool passchange = false)
        {
            if(Session["Username"] != null)
            {
                return View();
            }
            else return RedirectToAction("Login", "Admin");
        }

        private bool ContainsChars(string text, char[] array)
        {
            for(int i=0; i<array.Length; i++)
            {
                if (text.Contains(array[i])) return true;
            }
            return false;
        }
        private bool ContainsChars(string text, int fchar, int lchar)
        {
            for (int i = fchar; i <= lchar; i++)
            {
                if (text.Contains((char)i)) return true;
            }
            return false;
        }

        private SResponse PasswordIsValid(string password, string username)
        {
            if (hasher.Verify(password, database.Users.Find(username).Pass))
            {
                return new SResponse(false, "Nowe hasło nie może być takie samo");
            }
            if (password.Contains(username))
            {
                return new SResponse(false, "Hasło nie może zawierać nazwy użytkownika");
            }
            if (password == null || password.Length < 6)
            {
                return new SResponse(false, "Hasło nie może być krótsze niż 6 znaków");
            }
            if (!ContainsChars(password, 65, 90) && !ContainsChars(password, 97, 122))
            {
                return new SResponse(false, "Hasło musi zawierać litery");
            }
            if (!ContainsChars(password, 48, 57))
            {
                return new SResponse(false, "Hasło musi zawierać liczby");
            }
            if (!ContainsChars(password, new[] { '.', ',', '!', '#', '$', '%', '&', '-', '+', '=', '?', '*', '^' }))
            {
                return new SResponse(false, "Hasło musi zawierać znaki specjalne");
            }

            return new SResponse(true, "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PassChange form)
        {
            string username = Session["username"].ToString();

            // Current password
            if (!hasher.Verify(form.CurrentPass, database.Users.Find(username).Pass))
            {
                ViewBag.ValidationText1 = "Nieprawidłowe hasło";
                return View();
            }

            // New password
            SResponse response = PasswordIsValid(form.NewPass, username);
            if(!response.Good)
            {
                ViewBag.ValidationText2 = response.Message;
                return View();
            }
            
            // Repeat password
            if (form.NewPass != form.RepeatPass)
            {
                ViewBag.ValidationText3 = "Powtórzone hasło musi być takie samo";
                return View();
            }

            // Password change
            database.Users.Find(username).Pass = hasher.Hash(form.NewPass);
            database.SaveChanges();

            // Return view
            ModelState.Clear();
            ViewBag.Notification = "Hasło zostało poprawnie zmienione";
            return View();
        }
    }
}
