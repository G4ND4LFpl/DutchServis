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
    public class MatchesController : Controller
    {
        private DutchDatabaseEntities1 database = new DutchDatabaseEntities1();

        public ActionResult Index()
        {
            return RedirectToAction("Tournaments");
        }

        public ActionResult Tournaments()
        {
            // Make query
            var query = from tourn in database.Tournaments
                        where tourn.Type == "tournament"
                        select new TournamentInfo
                        {
                            Name = tourn.Name,
                            Date = tourn.StartDate,
                            Location = tourn.Location,
                            Theme = tourn.Theme,
                            Info = tourn.Info,
                            Img = tourn.ImgPath
                        };

            query = query.OrderByDescending(item => item.Date);

            // Return View
            return View(query);
        }
        
        public ActionResult TournamentInfo(string name)
        {
            // Games won player 1
            var player1_games = from games in database.Games
                                join matches in database.Matches
                                on games.MatchId equals matches.MatchId
                                join players in database.Players
                                on matches.Player1_Id equals players.PlayerId
                                group games by new
                                {
                                    matches.MatchId,
                                    players.Nickname
                                } into gruped
                                select new
                                {
                                    gruped.Key.MatchId,
                                    gruped.Key.Nickname,
                                    Points = gruped.Count(x => x.Win == 1)
                                };

            // Games won player 2
            var player2_games = from games in database.Games
                                join matches in database.Matches
                                on games.MatchId equals matches.MatchId
                                join players in database.Players
                                on matches.Player2_Id equals players.PlayerId
                                group games by new
                                {
                                    matches.MatchId,
                                    players.Nickname
                                } into gruped
                                select new
                                {
                                    gruped.Key.MatchId,
                                    gruped.Key.Nickname,
                                    Points = gruped.Count(x => x.Win == 2)
                                };

            // Get matches
            var matchlist = from matches in database.Matches
                            join tourn in database.Tournaments
                            on matches.TournamentId equals tourn.TournamentId
                            join player1 in player1_games
                            on matches.MatchId equals player1.MatchId
                            join player2 in player2_games
                            on matches.MatchId equals player2.MatchId
                            where tourn.Name == name
                            select new Match
                            {
                                Id = matches.MatchId,
                                Player1 = player1.Nickname,
                                PointsPlayer1 = player1.Points,
                                Player2 = player2.Nickname,
                                PointsPlayer2 = player2.Points,
                                PlayDate = matches.PlayDate,
                                FormatBo = matches.FormatBo
                            };

            // Make query
            var query = from tourn in database.Tournaments
                        where tourn.Name == name
                        select new TournamentInfo
                        {
                            Name = tourn.Name,
                            Date = tourn.StartDate,
                            Location = tourn.Location,
                            Theme = tourn.Theme,
                            Info = tourn.Info,
                            Img = tourn.ImgPath,
                            Matches = matchlist.ToList()
                        };

            // Return View
            return View(query.FirstOrDefault());
        }
        
        // GET: Matches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Games games = database.Games.Find(id);
            if (games == null)
            {
                return HttpNotFound();
            }
            return View(games);
        }

        // GET: Matches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Matches/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GameId,MatchId,PointsPlayer1,PointsPlayer2,MistakesPlayer1,MistakesPlayer2,Win,Opening,Dutch")] Games games)
        {
            if (ModelState.IsValid)
            {
                database.Games.Add(games);
                database.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(games);
        }

        // GET: Matches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Games games = database.Games.Find(id);
            if (games == null)
            {
                return HttpNotFound();
            }
            return View(games);
        }

        // POST: Matches/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GameId,MatchId,PointsPlayer1,PointsPlayer2,MistakesPlayer1,MistakesPlayer2,Win,Opening,Dutch")] Games games)
        {
            if (ModelState.IsValid)
            {
                database.Entry(games).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(games);
        }

        // GET: Matches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Games games = database.Games.Find(id);
            if (games == null)
            {
                return HttpNotFound();
            }
            return View(games);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Games games = database.Games.Find(id);
            database.Games.Remove(games);
            database.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
