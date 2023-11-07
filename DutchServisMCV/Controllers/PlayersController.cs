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
    public class PlayersController : Controller
    {
        DutchDatabaseEntities1 database = new DutchDatabaseEntities1();

        public ActionResult Index()
        {
            // Make query
            var query = from player in database.Players
                        join clan in database.Clans
                        on player.ClanId equals clan.ClanId 
                        into groupedclans
                        select new PlayerInfo
                        {
                            Nickname = player.Nickname,
                            Img = player.ImgPath,
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
            if (nickname == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if( database.Players.Where(item => item.Nickname == nickname).FirstOrDefault() == null) return HttpNotFound();

            // Ranking
            var rank = from res in database.TournamentResults
                       join player in database.Players
                       on res.PlayerId equals player.PlayerId
                       where player.Nickname == nickname
                       group res by player.Nickname into gres
                       select new
                       {
                           Id = gres.Key,
                           Sum = gres.Sum(item => item.RankingGet),
                       };

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
                            Img = player.ImgPath,
                            Clan = (grupedclans.FirstOrDefault().Name != null ? grupedclans.FirstOrDefault().Name : "Brak"),
                            Ranking = rank.FirstOrDefault().Sum + player.Rating.Value,
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

        public ActionResult Details()
        {
            // Return View
            return View();
        }
    }
}
