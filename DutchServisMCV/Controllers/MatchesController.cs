using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Logic;
using DutchServisMCV.Models;

namespace DutchServisMCV.Controllers
{
    public class MatchesController : BaseMatchesController
    {
        // Match
        public ActionResult Details(int? id)
        {
            if (id == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (database.Matches.Find(id) == null) return HttpNotFound();

            var gameList = database.Games.Where(game => game.MatchId == id);

            var gameSum1 = GamesSumByMatch(1, id);
            var gameSum2 = GamesSumByMatch(2, id);

            var match = from matches in database.Matches
                        join tourn in database.Tournaments
                        on matches.TournamentId equals tourn.TournamentId
                        join player1 in database.Players
                        on matches.Player1_Id equals player1.PlayerId
                        join points1 in gameSum1
                        on matches.MatchId equals points1.MatchId
                        join points2 in gameSum2
                        on matches.MatchId equals points2.MatchId
                        join player2 in database.Players
                        on matches.Player2_Id equals player2.PlayerId
                        where matches.MatchId == id
                        select new MatchData
                        {
                            Id = matches.MatchId,
                            Tournament = tourn.Name,
                            Player1 = player1.Nickname,
                            PointsPlayer1 = points1.Points + (matches.BonusGamePlayer1 == true ? 1 : 0),
                            BonusGamePlayer1 = matches.BonusGamePlayer1,
                            Player2 = player2.Nickname,
                            PointsPlayer2 = points2.Points + (matches.BonusGamePlayer1 == true ? 1 : 0),
                            BonusGamePlayer2 = matches.BonusGamePlayer2,
                            PlayDate = matches.PlayDate,
                            FormatBo = matches.FormatBo,
                            Games = gameList.ToList()
                        };

            // Return View
            return View(match.FirstOrDefault());
        }

        public ActionResult Create()
        {
            // Return View
            return View();
        }

        public ActionResult Edit()
        {
            // Return View
            return View();
        }
    }
}
