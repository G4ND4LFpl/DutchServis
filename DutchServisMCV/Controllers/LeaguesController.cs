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
    public class LeaguesController : MatchesController
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
                            Img = tourn.ImgPath
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
            var query = from tourn in database.Tournaments
                        where tourn.Name == name
                        select new LeagueInfo
                        {
                            Name = tourn.Name,
                            StartDate = tourn.StartDate,
                            EndDate = tourn.EndDate.Equals(null) ? DateTime.MinValue : (DateTime)tourn.EndDate,
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
        public new ActionResult Create(LeagueInfo league)
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
            return RedirectToAction("LeagueEdit", new { name = league.Name });
        }

        public new ActionResult Edit()
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            return View();
        }

    }
}