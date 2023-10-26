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

        private List<PlayerInfo> Get_PlayerList()
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
            List<PlayerInfo> plist = query.ToList();
            for (int i = 0; i < plist.Count(); i++)
            {
                if (plist.ElementAt(i).Ranking == null) plist.ElementAt(i).Ranking = plist.ElementAt(i).Rating;
                if (plist.ElementAt(i).Clan == null) plist.ElementAt(i).Clan = "";
            }

            // Return list
            return plist;
        }

        public ActionResult Index()
        {
            return View(Get_PlayerList());
        }
    }
}
