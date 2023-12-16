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
                            Player2 = player2.Nickname,
                            PointsPlayer2 = points2.Points + (matches.BonusGamePlayer1 == true ? 1 : 0),
                            PlayDate = matches.PlayDate,
                            FormatBo = matches.FormatBo,
                            BonusGamePlayer = matches.BonusGamePlayer1 == true ? 1 : (matches.BonusGamePlayer2 == true ? 2 : 0),
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

        private IQueryable<PlayerItem> GetPlayers(int id1, int id2)
        {
            return from players in database.Players
                   where players.PlayerId == id1 || players.PlayerId == id2
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
            ViewBag.PlayersSet = GetPlayerList(parent.TournamentId).ToList();
            Dictionary<int, string> formats = new Dictionary<int, string>
            {
                { 5, "Bo5" }, { 7, "Bo7" }, { 9, "Bo9" }, { 10, "Bo10" }, { 11, "Bo11" }, { 13, "Bo13" }
            };
            ViewBag.Formats = formats;
            ViewBag.Players = new List<PlayerItem> { null, null };

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

            // Notification
            if (TempData.ContainsKey("Notification"))
            {
                ViewBag.Notification = TempData["Notification"];
            }

            // Return View
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MatchData match)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Prepare Viewbag
            Tournaments tournament = database.Tournaments.Where(item => item.Name == match.Tournament).FirstOrDefault();
            ViewBag.PlayersSet = GetPlayerList(tournament.TournamentId).ToList();
            Dictionary<int, string> formats = new Dictionary<int, string>
            {
                { 5, "Bo5" }, { 7, "Bo7" }, { 9, "Bo9" }, { 10, "Bo10" }, { 11, "Bo11" }, { 13, "Bo13" }
            };
            ViewBag.Formats = formats;
            ViewBag.Players = GetPlayers(match.PlayerId1, match.PlayerId2).ToList();

            // Validation
            if (match.PlayerId1 == 0)
            {
                ViewBag.Player1ValidationMsg = "Pole \"Pierwszy gracz\" nie może być puste";
                return View(match);
            }

            if (match.PlayerId2 == 0)
            {
                ViewBag.Player2ValidationMsg = "Pole \"Drugi gracz\" nie może być puste";
                return View(match);
            }

            if (match.PlayerId1 == match.PlayerId2)
            {
                ViewBag.Player2ValidationMsg = "Gracz nie może grać sam ze sobą";
                return View(match);
            }

            if(match.PlayDate.Year == 1)
            {
                ViewBag.DateValidationMsg = "Pole Data nie może być puste";
                return View(match);
            }

            if (match.Opens == 0)
            {
                ViewBag.OpensValidationMsg = "Musisz ustawić gracza rozpoczynającego grę";
                return View(match);
            }

            // Add Match To Database
            Matches matchObject = new Matches
            {
                Player1_Id = match.PlayerId1,
                Player2_Id = match.PlayerId2,
                TournamentId = tournament.TournamentId,
                PlayDate = match.PlayDate,
                FormatBo = match.FormatBo,
                BonusGamePlayer1 = match.BonusGamePlayer == match.PlayerId1,
                BonusGamePlayer2 = match.BonusGamePlayer == match.PlayerId2
            };

            database.Matches.Add(matchObject);
            database.SaveChanges();

            // Add Games To Database
            for (int i = 0; i<match.Games.Count; i++)
            {
                Games game = match.Games[i];
                // Id
                game.MatchId = matchObject.MatchId;

                // Opening
                if (i % 2 == 0) game.Opening = (match.Opens == match.PlayerId1) ? 1 : 2;
                else game.Opening = (match.Opens == match.PlayerId1) ? 2 : 1;

                // Win
                int p1 = game.PointsPlayer1 ?? -1;
                int p2 = game.PointsPlayer2 ?? -1;
                if (p1 > p2 || (p1 == p2 && game.Dutch == 1)) game.Win = 2;
                else game.Win = 1;

                database.Games.Add(game);
            }
            database.SaveChanges();

            // Return
            TempData.Add("Notification", "Mecz został dodany");
            return RedirectToAction("Add", "Matches", new { tournament = tournament.Name});
        }

        public ActionResult Edit()
        {
            // Return View
            return View();
        }
    }
}
