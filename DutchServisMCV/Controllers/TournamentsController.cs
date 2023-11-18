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
    public class TournamentsController : BaseMatchesController
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
                            Players = playerslist.ToList()
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
                string path = Server.MapPath("~/Content/images/playerdata/") + tournament.File.FileName;

                try
                {
                    FileManager.Save(tournament.File, path);
                    tournament.Img = tournament.File?.FileName;
                }
                catch (OverrideException ex)
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
            return RedirectToAction("TournamentEdit", new { name = tournament.Name });
        }

        public ActionResult Edit(string name)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Validate adress
            if (name == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Tournaments t = database.Tournaments.Where(item => item.Name == name).FirstOrDefault();
            if (t == null) return HttpNotFound();

            // Prapare
            var player1_games = GamesSumByMatch(1);
            var player2_games = GamesSumByMatch(2);
            var matchlist = GetMatchList(name, player1_games, player2_games);
            var playerset = GetPlayerSet(name);

            var playerlist = from players in database.Players
                             where !(from other in database.PlayerSet
                                     join tournament in database.Tournaments
                                     on other.TournamentId equals tournament.TournamentId
                                     where tournament.Name == name
                                     select other
                                     ).Any(p => p.PlayerId == players.PlayerId)
                             select new PlayerItem
                             {
                                 Id = players.PlayerId,
                                 Nickname = players.Nickname
                             };
            ViewBag.Players = playerlist.OrderBy(item => item.Nickname).ToList();

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
            var playerlist = from players in database.Players
                             select new PlayerItem
                             {
                                 Id = players.PlayerId,
                                 Nickname = players.Nickname
                             };
            ViewBag.Players = playerlist.OrderBy(item => item.Nickname).ToList();

            if (tournament.Matches == null) tournament.Matches = new List<MatchData>();
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
                string path = Server.MapPath("~/Content/images/playerdata/") + tournament.File.FileName;

                try
                {
                    FileManager.Save(tournament.File, path);
                    tournament.Img = tournament.File?.FileName;
                }
                catch (OverrideException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View();
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
                Info = tournament.Info
            };
            database.Entry(tournamentObject).State = EntityState.Modified;

            // Add PlayerSet to Database
            foreach(PlayerTournItem playerItem in tournament.Players)
            {
                double? rank = playerItem.RankingBefore;
                if(rank == null || rank == 0.0)
                {
                    rank = database.Players.Where(p => p.PlayerId == playerItem.Id).FirstOrDefault().Rating;
                    var sum = (from set in database.PlayerSet
                               where set.PlayerId == playerItem.Id
                               group set by set.PlayerId into gres
                               select new
                               {
                                   Id = gres.Key,
                                   Sum = gres.Sum(item => item.RankingGet)
                               }
                               ).FirstOrDefault();
                    rank += (sum != null ? sum.Sum : 0.0);
                }

                PlayerSet player = new PlayerSet
                {
                    TournamentId = tournament.Id,
                    PlayerId = playerItem.Id,
                    Ranking = rank,
                    Place = playerItem.Place,
                    RankingGet = playerItem.RankingGet,
                    Prize = playerItem.Price
                };
  
                if (database.PlayerSet.Where(p => p.TournamentId == tournament.Id).Any(item => item.PlayerId != playerItem.Id))
                {
                    // Add item
                    database.PlayerSet.Add(player);
                }
                else
                {
                    // Update item
                    database.Entry(player).State = EntityState.Modified;
                }
            }
            // Remove from playerSet
            foreach (PlayerSet playerItem in database.PlayerSet.Where(p => p.TournamentId == tournament.Id))
            {
                if(tournament.Players.Where(item => item.Id == playerItem.PlayerId).ToList() != null)
                {
                    database.PlayerSet.Remove(playerItem);
                }
            }

            // Save chages
            database.SaveChanges();

            // Redirect
            return RedirectToAction("Info", new { name = tournament.Name });
        }
    }
}