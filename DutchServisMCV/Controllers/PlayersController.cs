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
    public class PlayersController : Controller
    {
        DutchDatabaseEntities database = new DutchDatabaseEntities();

        public ActionResult Index()
        {
            // Make query
            var query = from player in database.Players
                        join clan in database.Clans
                        on player.ClanId equals clan.ClanId
                        into groupedclans
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

                            Rating = player.Rating.Value,
                            Active = player.Active
                        };

            // Replace null values by default
            List<PlayerInfo> listPI = query.ToList();
            for (int i = 0; i < listPI.Count(); i++)
            {
                if (listPI.ElementAt(i).Ranking == null) listPI.ElementAt(i).Ranking = listPI.ElementAt(i).Rating;
                if (listPI.ElementAt(i).Clan == null) listPI.ElementAt(i).Clan = "";
            }

            // Return View
            return View(listPI);
        }

        public ActionResult Info(string nickname)
        {
            if (nickname == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Players p = database.Players.Where(item => item.Nickname == nickname).FirstOrDefault();
            if(p == null) return HttpNotFound();

            return View(p);
        }

        public ActionResult Details()
        {
            return View();
        }
    }
}
