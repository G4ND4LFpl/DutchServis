using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Models;
using DutchServisMCV.Models.GameNamespace;

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

        public ActionResult Leagues()
        {
            // Make query
            var query = from tourn in database.Tournaments
                        where tourn.Type == "league"
                        select new LeaugeInfo
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

        private IQueryable<PlayerTournItem> GetPlayerSet(string tournament)
        {
            return (from set in database.PlayerSet
                    join tourn in database.Tournaments
                    on set.TournamentId equals tourn.TournamentId
                    join players in database.Players
                    on set.PlayerId equals players.PlayerId
                    join res in (from res in database.TournamentResults
                                 join tourn in database.Tournaments
                                 on res.TournamentId equals tourn.TournamentId
                                 where tourn.Name == tournament
                                 select res
                                 )
                    on set.PlayerId equals res.PlayerId
                    where tourn.Name == tournament
                    select new PlayerTournItem
                    {
                        Nickname = players.Nickname,
                        RankingBefore = set.Ranking,
                        Place = res.Place,
                        RankingGet = res.RankingGet,
                        Price = res.Prize,
                    });
        }
        private IQueryable<PlayerLeagueItem> GetPlayerSet(string tournament, IQueryable<MatchData> matchlist)
        {
            var stat_table = from matches in (
                from matches in matchlist
                group matches by matches.Player1 into gruped
                select new
                {
                    gruped.Key,
                    Won = gruped.Count(x => x.PointsPlayer1 > x.PointsPlayer2),
                    Loose = gruped.Count(x => x.PointsPlayer1 < x.PointsPlayer2),
                    Draw = gruped.Count(x => x.PointsPlayer1 == x.PointsPlayer2)
                }).Union(
                    from matches in matchlist
                    group matches by matches.Player2 into gruped
                    select new
                    {
                        gruped.Key,
                        Won = gruped.Count(x => x.PointsPlayer2 > x.PointsPlayer1),
                        Loose = gruped.Count(x => x.PointsPlayer2 < x.PointsPlayer1),
                        Draw = gruped.Count(x => x.PointsPlayer2 == x.PointsPlayer1)
                    })
                        group matches by matches.Key into gruped
                        select new
                        {
                            Player = gruped.Key,
                            Points = gruped.Sum(x => x.Won * 3 + x.Draw),
                            Won = gruped.Sum(x => x.Won),
                            Loose = gruped.Sum(x => x.Loose),
                            Draw = gruped.Sum(x => x.Draw)
                        };

            return (from set in database.PlayerSet
                    join tourn in database.Tournaments
                    on set.TournamentId equals tourn.TournamentId
                    join players in database.Players
                    on set.PlayerId equals players.PlayerId
                    join res in (from res in database.TournamentResults
                                 join tourn in database.Tournaments
                                 on res.TournamentId equals tourn.TournamentId
                                 where tourn.Name == tournament
                                 select res
                                 )
                    on set.PlayerId equals res.PlayerId
                    join stats in stat_table
                    on players.Nickname equals stats.Player
                    where tourn.Name == tournament
                    select new PlayerLeagueItem
                    {
                        Nickname = players.Nickname,
                        Price = res.Prize,
                        Points = stats.Points,
                        Won = stats.Won,
                        Loose = stats.Loose,
                        Draw = stats.Draw
                    });
        }
        private IQueryable<GamesSum> GamesSumByMatch(int player, int? matchId = null)
        {
            return (from games in database.Games
                    join matches in database.Matches
                    on games.MatchId equals matches.MatchId
                    join players in database.Players
                    on (player == 1 ? matches.Player1_Id : matches.Player2_Id) equals players.PlayerId
                    group games by new
                    {
                        matches.MatchId,
                        players.Nickname
                    } into gruped
                    where matchId == null || gruped.Key.MatchId == matchId
                    select new GamesSum
                    {
                        MatchId = gruped.Key.MatchId,
                        Nickname = gruped.Key.Nickname,
                        Points = gruped.Count(x => x.Win == player)
                    });
        }
        private IQueryable<MatchData> GetMatchList(string tournament, IQueryable<GamesSum> games1, IQueryable<GamesSum> games2, int matchId = -1)
        {
            return (from matches in database.Matches
                    join tourn in database.Tournaments
                    on matches.TournamentId equals tourn.TournamentId
                    join player1 in games1
                    on matches.MatchId equals player1.MatchId
                    join player2 in games2
                    on matches.MatchId equals player2.MatchId
                    where tourn.Name == tournament && (matchId == -1 || matches.MatchId == matchId)
                    select new MatchData
                    {
                        Id = matches.MatchId,
                        Player1 = player1.Nickname,
                        PointsPlayer1 = player1.Points + (matches.BonusGamePlayer1 == true ? 1 : 0),
                        Player2 = player2.Nickname,
                        PointsPlayer2 = player2.Points + (matches.BonusGamePlayer2 == true ? 1 : 0),
                        PlayDate = matches.PlayDate,
                        FormatBo = matches.FormatBo
                    });
        }

        public ActionResult Link(string name)
        {
            if (name == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (database.Tournaments.Where(item => item.Name == name).FirstOrDefault() == null) return HttpNotFound();

            string target = database.Tournaments.Where(item => item.Name == name).FirstOrDefault().Type;

            if (target == "tournament") return RedirectToAction("TournamentInfo", new { name });
            if (target == "leauge") return RedirectToAction("LeagueInfo", new { name });

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult TournamentInfo(string name)
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

        public ActionResult LeagueInfo(string name)
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
                        select new LeaugeInfo
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

        /* WYGENEROWANE AUTOMATYCZNIE FUKCJE */

        // GET: Matches/Details/5
        public ActionResult DetailsAuto(int? id)
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
