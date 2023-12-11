using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Logic;
using DutchServisMCV.Models;
using DutchServisMCV.Models.GameNamespace;

namespace DutchServisMCV.Controllers
{
    public class LeaguesController : CompetitionController
    {
        public ActionResult Index()
        {
            // Make query
            var query = from tourn in database.Tournaments
                        where tourn.Type == "league"
                        select new LeagueInfo
                        {
                            Name = tourn.Name,
                            StartDate = tourn.StartDate,
                            EndDate = (DateTime)tourn.EndDate,
                            Info = tourn.Info,
                            Img = tourn.Img
                        };

            query = query.OrderByDescending(item => item.StartDate);

            // Return View
            return View(query);
        }

        public ActionResult Info(string name)
        {
            if (name == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (database.Tournaments.Where(item => item.Name == name).FirstOrDefault() == null) return HttpNotFound();

            // Games won player 1
            var player1_games = GamesSumByMatch(1);

            // Games won player 2
            var player2_games = GamesSumByMatch(2);

            // Get matches
            var matchlist = GetMatchList(name, player1_games, player2_games);

            // Get players
            var playerslist = GetPlayerSet(name, matchlist);

            // Make query
            var query = from league in database.Tournaments
                        where league.Name == name
                        select new LeagueInfo
                        {
                            Id = league.TournamentId,
                            Name = league.Name,
                            StartDate = league.StartDate,
                            EndDate = league.EndDate.Equals(null) ? DateTime.MinValue : (DateTime)league.EndDate,
                            Info = league.Info,
                            Img = league.Img,
                            Matches = matchlist.ToList(),
                            Players = playerslist.ToList(),
                            ShowResults = league.ShowResults.Value
                        };

            // Return View
            return View(query.FirstOrDefault());
        }

        public ActionResult Create()
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeagueInfo league)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Name Validation
            SResponse response = NameIsValid(league.Name);
            if (!response.Good)
            {
                ViewBag.NameValidationMsg = response.Message;
                return View();
            }
            league.Name = league.Name.Trim();

            // Start Data Validation
            if (!league.StartDate.HasValue)
            {
                ViewBag.StartDateValidationMsg = "Pole Data rozpoczęcia nie może być puste";
                return View(league);
            }

            // Time Validation
            if (!league.EndDate.HasValue)
            {
                ViewBag.EndDateValidationMsg = "Pole Data zakończenia nie może być puste";
                return View(league);
            }

            // Save File
            if (league.File != null)
            {
                string path = Server.MapPath("~/Content/images/tournamentdata/") + league.File.FileName;

                try
                {
                    FileManager.Save(league.File, path);
                    league.Img = league.File?.FileName;
                }
                catch (SaveFaildException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View(league);
                }
            }

            // Add To Database
            Tournaments item = new Tournaments
            {
                Name = league.Name,
                Type = "league",
                StartDate = league.StartDate.Value,
                EndDate = league.EndDate.Value,
                Info = league.Info
            };
            database.Tournaments.Add(item);
            database.SaveChanges();

            // Redirect
            return RedirectToAction("Edit", new { name = league.Name });
        }

        public ActionResult Edit(string name)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Validate adress
            if (name == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Tournaments t = database.Tournaments.Where(item => item.Name == name).FirstOrDefault();
            if (t == null) return HttpNotFound();

            // Prepare Viewbag
            ViewBag.Players = GetPlayerList(name).ToList();

            // Preparing Model
            var player1_games = GamesSumByMatch(1);
            var player2_games = GamesSumByMatch(2);
            var matchlist = GetMatchList(name, player1_games, player2_games);
            var playerset = GetPlayerSet(name, matchlist);

            LeagueInfo tourn = new LeagueInfo
            {
                Id = t.TournamentId,
                Name = t.Name,
                StartDate = t.StartDate.Date,
                EndDate = t.EndDate.Value.Date,
                Info = t.Info,
                Img = t.Img,
                Matches = matchlist.ToList(),
                Players = playerset.ToList(),
                ShowResults = t.ShowResults.Value
            };

            // Return View
            return View(tourn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeagueInfo league)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Prepare for return
            var playerlist = from players in database.Players
                             select new PlayerItem
                             {
                                 Id = players.PlayerId,
                                 Nickname = players.Nickname
                             };
            ViewBag.Players = playerlist.OrderBy(item => item.Nickname).ToList();

            if (league.Matches == null) league.Matches = new List<MatchInfo>();
            if (league.Players == null) league.Players = new List<PlayerLeagueItem>();

            // Name Validation
            SResponse response = NameIsValid(league.Name, league.Id);
            if (!response.Good)
            {
                ViewBag.NameValidationMsg = response.Message;
                return View(league);
            }
            league.Name = league.Name.Trim();

            // StartData Validation
            if (!league.StartDate.HasValue)
            {
                ViewBag.StartDataValidationMsg = "Pole Data rozpoczęcia nie może być puste";
                return View(league);
            }

            // EndData Validation
            if (!league.EndDate.HasValue)
            {
                ViewBag.EndDateValidationMsg = "Pole Data zakończenia nie może być puste";
                return View(league);
            }

            // File Validation
            response = FileManager.FileExtIsValid(league.File);
            if (!response.Good)
            {
                ViewBag.FileValidationMsg = response.Message;
                return View(league);
            }

            // Save File
            if (league.File != null)
            {
                string path = Server.MapPath("~/Content/images/tournamentdata/") + league.File.FileName;

                try
                {
                    FileManager.Save(league.File, path);
                    league.Img = league.File?.FileName;
                }
                catch (SaveFaildException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View();
                }
            }

            // Add tournament to Database
            Tournaments leagueObject = new Tournaments
            {
                TournamentId = league.Id,
                Name = league.Name,
                Type = "league",
                StartDate = league.StartDate.Value,
                EndDate = league.EndDate.Value,
                Img = league.Img,
                Info = league.Info,
                ShowResults = league.ShowResults
            };
            database.Entry(leagueObject).State = EntityState.Modified;

            // Update PlayerSet for tournament
            UpdatePlayerSet(league);

            // Save chages
            database.SaveChanges();

            // Redirect
            return RedirectToAction("Info", new { name = league.Name });
        }
    }
}