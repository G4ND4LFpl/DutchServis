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
    public class TournamentsController : CompetitionController
    {
        public ActionResult Index()
        {
            // Make query
            var query = from tourn in database.Tournaments
                        where tourn.Type == "tournament"
                        select new TournamentInfo
                        {
                            Id = tourn.TournamentId,
                            Name = tourn.Name,
                            DateTime = tourn.StartDate,
                            Location = tourn.Location,
                            Theme = tourn.Theme,
                            Info = tourn.Info,
                            Img = tourn.Img
                        };

            query = query.OrderByDescending(item => item.DateTime);

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
            var playerslist = GetPlayerSet(name);

            // Make query
            var query = from tourn in database.Tournaments
                        where tourn.Name == name
                        select new TournamentInfo
                        {
                            Id = tourn.TournamentId,
                            Name = tourn.Name,
                            DateTime = tourn.StartDate,
                            Location = tourn.Location,
                            Theme = tourn.Theme,
                            Info = tourn.Info,
                            Img = tourn.Img,
                            Matches = matchlist.ToList(),
                            Players = playerslist.ToList(),
                            ShowResults = tourn.ShowResults.Value
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
        public ActionResult Create(TournamentInfo tournament)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Name Validation
            SResponse response = NameIsValid(tournament.Name);
            if (!response.Good)
            {
                ViewBag.NameValidationMsg = response.Message;
                return View();
            }
            tournament.Name = tournament.Name.Trim();

            // Data Validation
            if (!tournament.Date.HasValue)
            {
                ViewBag.DateValidationMsg = "Pole Data nie może być puste";
                return View(tournament);
            }

            // Time Validation
            if (!tournament.Time.HasValue)
            {
                ViewBag.TimeValidationMsg = "Pole Godzina nie może być puste";
                return View(tournament);
            }

            // File Validation
            response = FileManager.FileExtIsValid(tournament.File);
            if (!response.Good)
            {
                ViewBag.FileValidationMsg = response.Message;
                return View(tournament);
            }

            // Save File
            if (tournament.File != null)
            {
                string path = Server.MapPath("~/Content/images/tournamentdata/") + tournament.File.FileName;

                try
                {
                    FileManager.Save(tournament.File, path);
                    tournament.Img = tournament.File?.FileName;
                }
                catch (SaveFaildException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View();
                }
            }

            // Add To Database
            DateTime dt = tournament.Date.Value.AddMinutes(tournament.Time.Value.Hour * 60 + tournament.Time.Value.Minute);
            Tournaments item = new Tournaments
            {
                Name = tournament.Name,
                Type = "tournament",
                StartDate = dt,
                Location = tournament.Location,
                Theme = tournament.Theme,
                Img = tournament.Img,
                Info = tournament.Info
            };
            database.Tournaments.Add(item);
            database.SaveChanges();

            // Redirect
            return RedirectToAction("Edit", new { name = tournament.Name });
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
            var playerset = GetPlayerSet(name);

            TournamentInfo tourn = new TournamentInfo
            {
                Id = t.TournamentId,
                Name = t.Name,
                Date = t.StartDate.Date,
                Time = t.StartDate,
                Location = t.Location,
                Theme = t.Theme,
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
        public ActionResult Edit(TournamentInfo tournament)
        {   
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Prepare for return
            ViewBag.Players = GetPlayerList().ToList();

            if (tournament.Matches == null) tournament.Matches = new List<MatchInfo>();
            if (tournament.Players == null) tournament.Players = new List<PlayerTournItem>();

            // Name Validation
            SResponse response = NameIsValid(tournament.Name, tournament.Id);
            if (!response.Good)
            {
                ViewBag.NameValidationMsg = response.Message;
                return View(tournament);
            }
            tournament.Name = tournament.Name.Trim();

            // Data Validation
            if (!tournament.Date.HasValue)
            {
                ViewBag.DateValidationMsg = "Pole Data nie może być puste";
                return View(tournament);
            }

            // Time Validation
            if (!tournament.Time.HasValue)
            {
                ViewBag.TimeValidationMsg = "Pole Godzina nie może być puste";
                return View(tournament);
            }

            // File Validation
            response = FileManager.FileExtIsValid(tournament.File);
            if (!response.Good)
            {
                ViewBag.FileValidationMsg = response.Message;
                return View(tournament);
            }

            // Save File
            if (tournament.File != null)
            {
                string path = Server.MapPath("~/Content/images/tournamentdata/") + tournament.File.FileName;

                try
                {
                    FileManager.Save(tournament.File, path);
                    tournament.Img = tournament.File?.FileName;
                }
                catch (SaveFaildException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View(tournament);
                }
            }

            // Add tournament to Database
            DateTime dt = tournament.Date.Value.AddMinutes(tournament.Time.Value.Hour * 60 + tournament.Time.Value.Minute);
            Tournaments tournamentObject = new Tournaments
            {
                TournamentId = tournament.Id,
                Name = tournament.Name,
                Type = "tournament",
                StartDate = dt,
                Location = tournament.Location,
                Theme = tournament.Theme,
                Img = tournament.Img,
                Info = tournament.Info,
                ShowResults = tournament.ShowResults
            };
            database.Entry(tournamentObject).State = EntityState.Modified;

            // Update PlayerSet for tournament
            UpdatePlayerSet(tournament);

            // Save chages
            database.SaveChanges();

            // Redirect
            return RedirectToAction("Info", new { name = tournament.Name });
        }
    }
}