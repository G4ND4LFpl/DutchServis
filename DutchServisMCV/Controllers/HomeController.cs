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

        public ActionResult PlayerList()
        {
            var query = from player in database.Players
                        join clan in database.Clans
                        on player.ClanId equals clan.ClanId
                        into groupedclans
                        where player.Active == true
                        select new PlayerInfo
                        {
                            Nickname = player.Nickname,
                            Img = player.ImgPath,
                            Clan = groupedclans.FirstOrDefault().Name,
                            Ranking = (from res in database.TournamentResults
                                        where res.PlayerId == player.PlayerId
                                        group res by res.PlayerId into gres
                                        select new
                                        {
                                           Id = gres.Key,
                                           Sum = gres.Sum(item => item.RankingGet),
                                        }
                                        ).FirstOrDefault().Sum + player.Rating.Value,
                                       
                            Rating = player.Rating.Value
                        };

            List<PlayerInfo> plist = query.OrderByDescending(x => x.Rating).ToList();

            for(int i=0; i< plist.Count();i++)
            {
                if (plist.ElementAt(i).Ranking == null) plist.ElementAt(i).Ranking = plist.ElementAt(i).Rating;
            }

            return View(plist);
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