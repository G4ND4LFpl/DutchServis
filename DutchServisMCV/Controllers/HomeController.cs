using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Models;
using System.Data;

namespace DutchServisMCV.Controllers
{
    public class HomeController : Controller
    {
        DataBaseManager dbmanager = new DataBaseManager();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users userInfo)
        {
            if (dbmanager.IsLoginCorrect(userInfo))
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

        public ActionResult PlayerList()
        {
            return View(dbmanager.Get_PlayerList(PlayerInfo.Attribute.Ranking, Order.DESC));
        }
    }
}

/*
string connectionString = ConfigurationManager.ConnectionStrings["DutchDatabaseEntities1"].ConnectionString;
connectionString = connectionString.Remove(0, connectionString.IndexOf("data source"));
connectionString = connectionString.Remove(connectionString.IndexOf("App"));
SqlConnection connection = new SqlConnection(connectionString);
connection.Open();
SqlCommand query = new SqlCommand("SELECT * FROM dbo.Players", connection);
SqlDataReader data = query.ExecuteReader();
connection.Close();

return View(data);
*/