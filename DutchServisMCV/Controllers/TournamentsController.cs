using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Logic;
using DutchServisMCV.Models;

namespace DutchServisMCV.Controllers
{
    public class TournamentsController : MatchesController
    {
        public ActionResult Index()
        {
            // Make query
            var query = from tourn in database.Tournaments
                        where tourn.Type == "tournament"
                        select new TournamentInfo
                        {
                            Name = tourn.Name,
                            DateTime = tourn.StartDate,
                            Location = tourn.Location,
                            Theme = tourn.Theme,
                            Info = tourn.Info,
                            Img = tourn.ImgPath
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
                            Name = tourn.Name,
                            DateTime = tourn.StartDate,
                            Location = tourn.Location,
                            Theme = tourn.Theme,
                            Info = tourn.Info,
                            Img = tourn.ImgPath,
                            Matches = matchlist.ToList(),
                            Players = playerslist.ToList()
                        };

            // Return View
            return View(query.FirstOrDefault());
        }

        public new ActionResult Create()
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Create(TournamentInfo tournament)
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

            // Add To Database
            DateTime dt = tournament.Date.Value.AddMinutes(tournament.Time.Value.Hour * 60 + tournament.Time.Value.Minute);

            Tournaments item = new Tournaments
            {
                Name = tournament.Name,
                Type = "tournament",
                StartDate = dt,
                Location = tournament.Location,
                Theme = tournament.Theme,
                Info = tournament.Info
            };
            database.Tournaments.Add(item);
            database.SaveChanges();

            // Redirect
            return RedirectToAction("TournamentEdit", new { name = tournament.Name });
        }

        public new ActionResult Edit()
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            return View();
        }
    }
}