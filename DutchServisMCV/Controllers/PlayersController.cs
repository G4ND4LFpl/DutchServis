using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Models;
using DutchServisMCV.Logic;

namespace DutchServisMCV.Controllers
{
    public class PlayersController : Controller
    {
        DutchDatabaseEntities1 database = new DutchDatabaseEntities1();
        const double baseRating = 1000;

        public ActionResult Index()
        {
            if (HttpContext.Request.Path.EndsWith("/"))
            {
                return RedirectToAction("Index");
            }

            // Make query
            var query = from player in database.Players
                        join clan in database.Clans
                        on player.ClanId equals clan.ClanId 
                        into groupedclans
                        select new PlayerInfo
                        {
                            Nickname = player.Nickname,
                            Img = player.Img,
                            Clan = groupedclans.FirstOrDefault().Name,
                            Ranking = (from res in database.TournamentResults
                                       where res.PlayerId == player.PlayerId
                                       group res by res.PlayerId into gres
                                       select new
                                       {
                                           Id = gres.Key,
                                           Sum = gres.Sum(item => item.RankingGet),
                                       }
                                        ).FirstOrDefault().Sum + player.Rating.Value,

                            Rating = player.Rating.Value,
                            Active = player.Active
                        };

            // Replace null values by default
            List<PlayerInfo> listPI = query.ToList();
            for (int i = 0; i < listPI.Count(); i++)
            {
                if (listPI.ElementAt(i).Ranking == null) listPI.ElementAt(i).Ranking = listPI.ElementAt(i).Rating;
                if (listPI.ElementAt(i).Clan == null) listPI.ElementAt(i).Clan = "";
            }

            // Return View
            return View(listPI);
        }

        private IQueryable<Games> SelectWhere(string nickname, bool win)
        {
            if (win)
            {
                return (from games in database.Games
                        join matches in database.Matches
                        on games.MatchId equals matches.MatchId
                        join players in database.Players
                        on matches.Player1_Id equals players.PlayerId
                        where players.Nickname == nickname && games.Win == 1
                        select games).Union(
                            from games in database.Games
                            join matches in database.Matches
                            on games.MatchId equals matches.MatchId
                            join players in database.Players
                            on matches.Player2_Id equals players.PlayerId
                            where players.Nickname == nickname && games.Win == 1
                            select games
                       );
            }
            else
            {
                return (from games in database.Games
                        join matches in database.Matches
                        on games.MatchId equals matches.MatchId
                        join players in database.Players
                        on matches.Player1_Id equals players.PlayerId
                        where players.Nickname == nickname
                        select games).Union(
                            from games in database.Games
                            join matches in database.Matches
                            on games.MatchId equals matches.MatchId
                            join players in database.Players
                            on matches.Player2_Id equals players.PlayerId
                            where players.Nickname == nickname
                            select games
                       );
            }
        }
        private IQueryable<Games> SelectWhereOpen(string nickname, bool win)
        {
            if (win)
            {
                return (from games in database.Games
                        join matches in database.Matches
                        on games.MatchId equals matches.MatchId
                        join players in database.Players
                        on matches.Player1_Id equals players.PlayerId
                        where players.Nickname == nickname && games.Opening == 1 && games.Win == 1
                        select games).Union(
                            from games in database.Games
                            join matches in database.Matches
                            on games.MatchId equals matches.MatchId
                            join players in database.Players
                            on matches.Player2_Id equals players.PlayerId
                            where players.Nickname == nickname && games.Opening == 2 && games.Win == 1
                            select games
                       );
            }
            else
            {
                return (from games in database.Games
                        join matches in database.Matches
                        on games.MatchId equals matches.MatchId
                        join players in database.Players
                        on matches.Player1_Id equals players.PlayerId
                        where players.Nickname == nickname && games.Opening == 1
                        select games).Union(
                            from games in database.Games
                            join matches in database.Matches
                            on games.MatchId equals matches.MatchId
                            join players in database.Players
                            on matches.Player2_Id equals players.PlayerId
                            where players.Nickname == nickname && games.Opening == 2
                            select games
                       );
            }
        }
        private IQueryable<Games> SelectWhereDutch(string nickname, bool win)
        {
            if (win)
            {
                return (from games in database.Games
                        join matches in database.Matches
                        on games.MatchId equals matches.MatchId
                        join players in database.Players
                        on matches.Player1_Id equals players.PlayerId
                        where players.Nickname == nickname && games.Dutch == 1 && games.Win == 1
                        select games).Union(
                            from games in database.Games
                            join matches in database.Matches
                            on games.MatchId equals matches.MatchId
                            join players in database.Players
                            on matches.Player2_Id equals players.PlayerId
                            where players.Nickname == nickname && games.Dutch == 2 && games.Win == 1
                            select games
                       );
            }
            else
            {
                return (from games in database.Games
                        join matches in database.Matches
                        on games.MatchId equals matches.MatchId
                        join players in database.Players
                        on matches.Player1_Id equals players.PlayerId
                        where players.Nickname == nickname && games.Dutch == 1
                        select games).Union(
                            from games in database.Games
                            join matches in database.Matches
                            on games.MatchId equals matches.MatchId
                            join players in database.Players
                            on matches.Player2_Id equals players.PlayerId
                            where players.Nickname == nickname && games.Dutch == 2
                            select games
                       );
            }
        }

        public ActionResult Info(string nickname)
        {
            // Validate adress
            if (nickname == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if( database.Players.Where(item => item.Nickname == nickname).FirstOrDefault() == null) return HttpNotFound();

            // Ranking
            var rank_query = from res in database.TournamentResults
                             join player in database.Players
                             on res.PlayerId equals player.PlayerId
                             where player.Nickname == nickname
                             group res by player.Nickname into gres
                             select new
                             {
                                 Id = gres.Key,
                                 Sum = gres.Sum(item => item.RankingGet),
                             };

            double rank = 0;
            if (rank_query.Count() != 0)
            {
                rank = rank_query.FirstOrDefault().Sum;
            }

            // Win ration
            var gamesTot = SelectWhere(nickname, false);
            var gamesWin = SelectWhere(nickname, true);

            // Opening win ration
            var openTot = SelectWhereOpen(nickname, false);
            var openWin = SelectWhereOpen(nickname, true);

            // Dutch win ratio
            var dutchTot = SelectWhereDutch(nickname, false);
            var dutchWin = SelectWhereDutch(nickname, true);

            // Final query
            var query = from player in database.Players
                        join clan in database.Clans
                        on player.ClanId equals clan.ClanId
                        into grupedclans
                        where player.Nickname == nickname
                        select new PlayerData
                        {
                            Nickname = player.Nickname,
                            Name = player.Name,
                            Surname = player.Surname,
                            JoinDate = (
                                player.JoinDate != null ? (DateTime)player.JoinDate : DateTime.Now
                            ),
                            Img = player.Img,
                            Clan = grupedclans.FirstOrDefault().Name ?? "Brak",
                            Ranking = rank + player.Rating.Value,
                            Winratio = gamesTot.Count() != 0 ? (double?)Math.Floor((double)gamesWin.Count() / gamesTot.Count() * 100.0) : null,
                            WinGames = gamesWin.Count(),
                            TotalGames = gamesTot.Count(),
                            OpenWinration = openTot.Count() != 0 ? (double?)Math.Floor((double)openWin.Count() / openTot.Count() * 100.0) : null,
                            WinOpenings = openWin.Count(),
                            Openings = openTot.Count(),
                            DutchWinration = dutchTot.Count() != 0 ? (double?)Math.Floor((double)dutchWin.Count() / dutchTot.Count() * 100.0) : null,
                            WinDutchs = dutchWin.Count(),
                            Dutchs = dutchTot.Count(),
                            Rating = player.Rating.Value,
                            Status = (player.Active == true ? "Aktywny" : "Nieaktywny")
                        };

            // Return View
            return View(query.FirstOrDefault());
        }

        private SResponse NicknameIsValid(string nickname, int id = -1)
        {
            if (nickname == null || nickname.Replace(" ", "") == "")
            {
                return new SResponse(false, "Pole Nick nie może być puste");
            }

            var repetitions = from players in database.Players
                              where players.Nickname == nickname.Trim() && players.PlayerId != id
                              select players;
            if (repetitions.Count() != 0)
            {
                return new SResponse(false, "Pole Nick musi być unikalne");
            }

            return new SResponse(true, "");
        }
        private SResponse FileExtIsValid(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string ext = Path.GetExtension(file.FileName);
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                {
                    return new SResponse(false, "Przesłany plik nie posiada akceptowanego rozszerzenia");
                }
            }
            return new SResponse(true, "");
        }

        public ActionResult Add()
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            ViewBag.Clans = database.Clans;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Players player)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            ViewBag.Clans = database.Clans;

            // Nickname Validation
            SResponse response = NicknameIsValid(player.Nickname);
            if (!response.Good)
            {  
                ViewBag.NickValidationMsg = response.Message;
                return View();
            }
            player.Nickname = player.Nickname.Trim();

            // File Validation
            response = FileExtIsValid(player.File);
            if(!response.Good)
            {
                ViewBag.FileValidationMsg = response.Message;
                return View();
            }

            // Save File
            if (player.File != null)
            {
                string path = Server.MapPath("~/Content/images/playerdata/") + player.File.FileName;

                try
                {
                    FileManager.Save(player.File, path);
                    player.Img = player.File?.FileName;
                }
                catch (OverrideException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View();
                }
            }

            // Add To Database
            player.JoinDate = DateTime.Now;
            player.Rating = baseRating;

            database.Players.Add(player);
            database.SaveChanges();

            // Return View
            ModelState.Clear();
            ViewBag.Notification = "Gracz został dodany";
            return View();
        }

        public ActionResult Edit(string nickname)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Validate adress
            if (nickname == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Players p = database.Players.Where(item => item.Nickname == nickname).FirstOrDefault();
            if (p == null) return HttpNotFound();

            // Prepare
            ViewBag.Clans = database.Clans;

            // Return View
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Players player)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            ViewBag.Clans = database.Clans;

            // Nickname Validation
            SResponse response = NicknameIsValid(player.Nickname, player.PlayerId);
            if (!response.Good)
            {
                ViewBag.NickValidationMsg = response.Message;
                return View(player);
            }
            player.Nickname = player.Nickname.Trim();

            // File Validation
            response = FileExtIsValid(player.File);
            if (!response.Good)
            {      
                ViewBag.FileValidationMsg = response.Message;
                return View(player);
            }

            // Data Validation
            if (!player.JoinDate.HasValue)
            {
                ViewBag.DateValidationMsg = "Pole Data dołączenia nie może być puste";
                return View(player);
            }

            // Rating Validation
            if (!player.Rating.HasValue)
            {
                ViewBag.RatingValidationMsg = "Nieprawidłowa wartość Pola Rating. Prawidłowym separatorem części dziesiętnych jest przecinek";
                return View(player);
            }

            // Save File
            if (player.File != null)
            {
                string path = Server.MapPath("~/Content/images/playerdata/") + player.File.FileName;

                try
                {
                    FileManager.Save(player.File, path);
                    if (player.Img != null)
                    {
                        FileManager.Remove(Server.MapPath("~/Content/images/playerdata/") + player.Img);
                    }
                    player.Img = player.File.FileName;
                }
                catch (OverrideException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View(player);
                }
            }

            // Edit In Database
            database.Entry(player).State = EntityState.Modified;
            database.SaveChanges();

            // Redirect
            return RedirectToAction("Info", new { nickname = player.Nickname });
        }
    }
}
