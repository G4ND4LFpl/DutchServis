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
        string currentCol = "Ranking";
        Order currentOrder = Order.DESC;

        enum Order
        {
            ASC,
            DESC
        }

        private Order SwitchOrder()
        {
            if (currentOrder == Order.ASC) return Order.DESC;
            else return Order.ASC;
        }

        private List<PlayerInfo> Get_PlayerList(string sortAtt, Order order, bool onlyActive = true, string filter = null)
        {
            IQueryable<Players> playerFilter = null;

            // Only active
            if (onlyActive)
            {
                playerFilter = from player in database.Players
                               where player.Active == true
                               select player;
            }
            else
            {
                playerFilter = from player in database.Players
                               select player;
            }

            // Filter nickname
            if (filter != null)
            {
                playerFilter = from player in playerFilter
                               where player.Nickname.Contains(filter)
                               select player;
            }

            // Make query
            var query = from player in playerFilter
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

                            Rating = player.Rating.Value
                        };

            // Replace null values by default
            List<PlayerInfo> plist = query.ToList();
            for (int i = 0; i < plist.Count(); i++)
            {
                if (plist.ElementAt(i).Ranking == null) plist.ElementAt(i).Ranking = plist.ElementAt(i).Rating;
            }

            // Sort
            switch (sortAtt)
            {
                case "Player":
                    {
                        if (order == Order.ASC) plist.Sort((x, y) => x.Nickname.CompareTo(y.Nickname));
                        else plist.Sort((x, y) => -(x.Nickname.CompareTo(y.Nickname)));
                        break;
                    }
                case "Ranking":
                    {
                        if (order == Order.ASC) plist.Sort((x, y) => x.Ranking.Value.CompareTo(y.Ranking.Value));
                        else plist.Sort((x, y) => -(x.Ranking.Value.CompareTo(y.Ranking.Value)));
                        break;
                    }
                case "Rating":
                    {
                        if (order == Order.ASC) plist.Sort((x, y) => x.Rating.Value.CompareTo(y.Rating.Value));
                        else plist.Sort((x, y) => -(x.Rating.Value.CompareTo(y.Rating.Value)));
                        break;
                    }
            }

            return plist;
        }

        public ActionResult Index()
        {
            currentCol = "Ranking";
            currentOrder = SwitchOrder();
            return View(Get_PlayerList(currentCol, currentOrder));
        }

        [HttpGet]
        public ActionResult Index(string sortColumn)
        {
            currentCol = sortColumn;
            currentOrder = SwitchOrder();
            return View(Get_PlayerList(currentCol, currentOrder));
        }


    }
}
