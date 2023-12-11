using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Logic;
using DutchServisMCV.Models;
using DutchServisMCV.Models.GameNamespace;

namespace DutchServisMCV.Controllers
{
    public class MatchesController : CompetitionController
    {
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

        private IQueryable<PlayerItem> GetPlayerList(int tournamentId)
        {
            return from players in database.Players
                   join set in database.PlayerSet
                   on players.PlayerId equals set.PlayerId
                   where set.TournamentId == tournamentId
                   select new PlayerItem
                   {
                       Id = players.PlayerId,
                       Nickname = players.Nickname
                   };
        }

        public ActionResult Add(string tournament)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Validate adress
            if (tournament == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Tournaments parent = database.Tournaments.Where(t => t.Name == tournament).FirstOrDefault();
            if (parent == null) return HttpNotFound();

            // Prepare Viewbag
            ViewBag.Players = GetPlayerList(parent.TournamentId).ToList();

            // Preparing Model
            MatchData data = new MatchData
            {
                TournamentId = parent.TournamentId,
                Tournament = parent.Name,
            };
            if (parent.Type == "tournament")
            {
                data.PlayDate = parent.StartDate;
            }

            // Return View
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MatchData match)
        {
            // Brakujące dane
            // bonus game 1/2
            // tournamentId ? (mamy tournament Name)

            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Prepare Viewbag
            ViewBag.Players = GetPlayerList(match.TournamentId).ToList();

            return View(match);
        }

        public ActionResult Edit()
        {
            // Return View
            return View();
        }
    }
}
