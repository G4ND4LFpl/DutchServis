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
using DutchServisMCV.Models.GameNamespace;

namespace DutchServisMCV.Controllers
{
    public class PlayersController : DataController
    {
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
                            Ranking = player.Rating.Value + (
                                from set in database.PlayerSet
                                where set.PlayerId == player.PlayerId
                                group set by set.PlayerId into gres
                                select new
                                {
                                    Id = gres.Key,
                                    Sum = gres.Sum(item => item.RankingGet),
                                }
                            ).FirstOrDefault().Sum,
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

        private IQueryable<PlayerGame> GetPlayerGames(string player)
        {
            return (
                from games in database.Games
                join matches in database.Matches on games.MatchId equals matches.MatchId
                join players in database.Players on matches.Player1_Id equals players.PlayerId
                where players.Nickname == player
                select new PlayerGame
                {
                    Nickname = players.Nickname,
                    Points = games.PointsPlayer1 ?? 0,
                    Mistakes = games.MistakesPlayer1 ?? 0,
                    Win = games.Win == 1,
                    Open = games.Opening == 1,
                    Dutch = games.Dutch == 1,
                    Clean = games.PointsPlayer1 == null
                }).Union(
                from games in database.Games
                join matches in database.Matches on games.MatchId equals matches.MatchId
                join players in database.Players on matches.Player2_Id equals players.PlayerId
                where players.Nickname == player
                select new PlayerGame
                {
                    Nickname = players.Nickname,
                    Points = games.PointsPlayer2 ?? 0,
                    Mistakes = games.MistakesPlayer2 ?? 0,
                    Win = games.Win == 2,
                    Open = games.Opening == 2,
                    Dutch = games.Dutch ==2,
                    Clean = games.PointsPlayer2 == null
                });
        }
        private double? GetPointsAvarage(IQueryable<PlayerGame> query)
        {
            if (query.Count() == 0) return null;

            double sum = (from table in query
                          group table by table.Nickname into gruped
                          select new
                          {
                              Sum = gruped.Sum(item => item.Points)
                          }).FirstOrDefault().Sum;

            return Math.Round(sum / query.Count(), 2);
        }

        public ActionResult Info(string nickname)
        {
            // Validate adress
            if (nickname == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if( database.Players.Where(item => item.Nickname == nickname).FirstOrDefault() == null) return HttpNotFound();

            // Ranking
            var rank_query = from set in database.PlayerSet
                             join player in database.Players
                             on set.PlayerId equals player.PlayerId
                             where player.Nickname == nickname
                             group set by player.Nickname into gres
                             select new
                             {
                                 Id = gres.Key,
                                 Sum = gres.Sum(item => item.RankingGet),
                             };

            double? rank = 0;
            if (rank_query.Count() != 0)
            {
                rank = rank_query.FirstOrDefault().Sum;
            }

            // Get games for player
            IQueryable<PlayerGame> games = GetPlayerGames(nickname);

            // Win ration
            int totGames = games.Count();
            int winGames = games.Where(x => x.Win).Count();

            // Opening win ration
            int totOpenings = games.Where(x => x.Open).Count();
            int winOpenings = games.Where(x => x.Open && x.Win).Count();

            // Dutch win ratio
            int totDutches = games.Where(x => x.Dutch).Count();
            int winDutches = games.Where(x => x.Dutch && x.Win).Count();

            // Avarage
            double? avg = GetPointsAvarage(games);
            double? avgWin = GetPointsAvarage(games.Where(x => x.Win));
            double? avgLoose = GetPointsAvarage(games.Where(x => !x.Win));

            // Clear Board
            int clearboard = games.Where(x => x.Clean).Count();

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
                            Id = player.PlayerId,
                            JoinDate = (
                                player.JoinDate != null ? (DateTime)player.JoinDate : DateTime.Now
                            ),
                            Img = player.Img,
                            Clan = grupedclans.FirstOrDefault().Name ?? "Brak",
                            Ranking = rank + player.Rating.Value,
                            Rating = player.Rating.Value,
                            Status = (player.Active == true ? "Aktywny" : "Nieaktywny"),
                            Stats = new Statistics
                            {
                                Games = new Stat
                                {
                                    Percentage = totGames != 0 ? (double?)Math.Floor((double)winGames / totGames * 100.0) : null,
                                    Win = winGames,
                                    Total = totGames
                                },
                                Openings = new Stat
                                {
                                    Percentage = totOpenings != 0 ? (double?)Math.Floor((double)winOpenings / totOpenings * 100.0) : null,
                                    Win = winOpenings,
                                    Total = totOpenings
                                },
                                Dutches = new Stat
                                {
                                    Percentage = totDutches != 0 ? (double?)Math.Floor((double)winDutches / totDutches * 100.0) : null,
                                    Win = winDutches,
                                    Total = totDutches
                                },
                                AvgPoints = avg,
                                AvgPointsWin = avgWin,
                                AvgPointsLoose = avgLoose,
                                ClearBoards = clearboard
                            }
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
            response = FileManager.FileExtIsValid(player.File);
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
                catch (SaveFaildException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View();
                }
            }

            // Add To Database
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
            response = FileManager.FileExtIsValid(player.File);
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
                catch (SaveFaildException ex)
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
