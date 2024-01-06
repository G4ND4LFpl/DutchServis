using System;
using System.Collections.Generic;
using System.Data;
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
    public class CompetitionController : DataController
    {
        // Redirect Function
        public ActionResult Link(string name)
        {
            if (name == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (database.Tournaments.Where(item => item.Name == name).FirstOrDefault() == null) return HttpNotFound();

            string target = database.Tournaments.Where(item => item.Name == name).FirstOrDefault().Type;

            if (target == "tournament") return RedirectToAction("Info", "Tournaments", new { name });
            if (target == "league") return RedirectToAction("Info", "Leagues", new { name });

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // Sql Query Functions
        protected IQueryable<PlayerTournItem> GetPlayerSet(string tournament)
        {
            return (from set in database.PlayerSet
                    join tourn in database.Tournaments
                    on set.TournamentId equals tourn.TournamentId
                    join players in database.Players
                    on set.PlayerId equals players.PlayerId
                    where tourn.Name == tournament
                    select new PlayerTournItem
                    {
                        Id = players.PlayerId, 
                        Nickname = players.Nickname,
                        RankingBefore = set.Ranking,
                        Place = set.Place,
                        RankingGet = set.RankingGet,
                        Price = set.Prize
                    });
        }
        protected IQueryable<PlayerLeagueItem> GetPlayerSet(string tournament, IQueryable<MatchInfo> matchlist)
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
                    join stats in stat_table
                    on players.Nickname equals stats.Player into grouped
                    from stats in grouped.DefaultIfEmpty()
                    where tourn.Name == tournament
                    select new PlayerLeagueItem
                    {
                        Id = players.PlayerId,
                        Nickname = players.Nickname,
                        Price = set.Prize,
                        Points = stats != null ? stats.Points : 0,
                        Won = stats != null ? stats.Won : 0,
                        Loose = stats != null ? stats.Loose : 0,
                        Draw = stats != null ? stats.Draw : 0
                    });
        }
        protected IQueryable<GamesSum> GamesSumByMatch(int player, int? matchId = null)
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
        protected IQueryable<MatchInfo> GetMatchList(string tournament, IQueryable<GamesSum> games1, IQueryable<GamesSum> games2, int matchId = -1)
        {
            return (from matches in database.Matches
                    join tourn in database.Tournaments
                    on matches.TournamentId equals tourn.TournamentId
                    join player1 in games1
                    on matches.MatchId equals player1.MatchId
                    join player2 in games2
                    on matches.MatchId equals player2.MatchId
                    where tourn.Name == tournament && (matchId == -1 || matches.MatchId == matchId)
                    select new MatchInfo
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

        protected IQueryable<PlayerItem> GetPlayerList()
        {
            var playerlist = from players in database.Players
                             select new PlayerItem
                             {
                                 Id = players.PlayerId,
                                 Nickname = players.Nickname
                             };
            return playerlist.OrderBy(item => item.Nickname);
        }
        protected IQueryable<PlayerItem> GetPlayerList(string tournamentName)
        {
            var playerlist = from players in database.Players
                             where !(from other in database.PlayerSet
                                     join tournament in database.Tournaments
                                     on other.TournamentId equals tournament.TournamentId
                                     where tournament.Name == tournamentName
                                     select other
                                     ).Any(p => p.PlayerId == players.PlayerId)
                             select new PlayerItem
                             {
                                 Id = players.PlayerId,
                                 Nickname = players.Nickname
                             };
            return playerlist.OrderBy(item => item.Nickname);
        }

        // Database Update Functions
        private void AddOrUpdate(PlayerSet player, int competitionId)
        {
            if (!database.PlayerSet.AsNoTracking().Where(p => p.TournamentId == competitionId).Any(item => item.PlayerId == player.PlayerId))
            {
                // Add item
                database.PlayerSet.Add(player);
            }
            else
            {
                // Update item
                player.EntryId = database.PlayerSet.AsNoTracking().Where(
                    item => item.TournamentId == competitionId && item.PlayerId == player.PlayerId
                    ).FirstOrDefault().EntryId;
                database.Entry(player).State = EntityState.Modified;
            }
        }
        protected void UpdatePlayerSet(CompetitionInfo<PlayerTournItem> competition)
        {
            foreach (PlayerItem playerItem in competition.Players)
            {
                PlayerTournItem tournamentItem = playerItem as PlayerTournItem;

                // Prepare rank value
                double? rank = tournamentItem.RankingBefore;
                if (rank == null || rank == 0.0)
                {
                    rank = database.Players.Where(p => p.PlayerId == tournamentItem.Id).FirstOrDefault().Rating;
                    var sum = (from set in database.PlayerSet
                               where set.PlayerId == tournamentItem.Id
                               group set by set.PlayerId into gres
                               select new
                               {
                                   Id = gres.Key,
                                   Sum = gres.Sum(item => item.RankingGet)
                               }
                               ).FirstOrDefault();
                    rank += (sum != null ? sum.Sum : 0.0);
                }

                // Create PlayerSet item
                PlayerSet player = new PlayerSet
                {
                    TournamentId = competition.Id,
                    PlayerId = tournamentItem.Id,
                    Ranking = rank,
                    Place = tournamentItem.Place,
                    RankingGet = tournamentItem.RankingGet,
                    Prize = tournamentItem.Price
                };

                // Add or Update Database
                AddOrUpdate(player, competition.Id);
            }

            // Removing items deleted in edition
            foreach (PlayerSet playerItem in database.PlayerSet.Where(p => p.TournamentId == competition.Id))
            {
                if (!competition.Players.Any(item => item.Id == playerItem.PlayerId))
                {
                    database.PlayerSet.Remove(playerItem);
                }
            }
        }
        protected void UpdatePlayerSet(CompetitionInfo<PlayerLeagueItem> competition)
        {
            foreach (PlayerItem playerItem in competition.Players)
            {
                PlayerLeagueItem leagueItem = playerItem as PlayerLeagueItem;

                // Create PlayerSet item
                PlayerSet player = new PlayerSet
                {
                    TournamentId = competition.Id,
                    PlayerId = leagueItem.Id,
                    Prize = leagueItem.Price
                };

                // Add or Update Database
                AddOrUpdate(player, competition.Id);
            }

            // Removing items deleted in edition
            foreach (PlayerSet playerItem in database.PlayerSet.Where(p => p.TournamentId == competition.Id))
            {
                if (!competition.Players.Any(item => item.Id == playerItem.PlayerId))
                {
                    database.PlayerSet.Remove(playerItem);
                }
            }
        }

        protected void UpdatePlyersRating(int player1Id, int player2Id, int wonby1, int wonby2, int won)
        {
            int p1dif = 0, p2dif = 0;

            if (won == 1)
            {
                p1dif = 9;
                p2dif = -9;
            }
            if (won == 2)
            {
                p1dif = -9;
                p2dif = 9;
            }

            p1dif += (wonby1 - wonby2);
            p2dif += (wonby2 - wonby1);

            Players player1 = database.Players.Find(player1Id);
            Players player2 = database.Players.Find(player2Id);

            int ratingdifference = (int)Math.Round((player1.Rating.Value - player2.Rating.Value)/100);

            player1.Rating += (p1dif - ratingdifference);
            database.Entry(player1).State = EntityState.Modified;

            player2.Rating += (p2dif + ratingdifference);
            database.Entry(player2).State = EntityState.Modified;
        }

        // Validation Functions
        protected SResponse NameIsValid(string name, int id = -1)
        {
            if (name == null || name.Replace(" ", "") == "")
            {
                return new SResponse(false, "Pole Nazwa nie może być puste");
            }

            var repetitions = from tourn in database.Tournaments
                              where tourn.Name == name.Trim() && tourn.TournamentId != id
                              select tourn;
            if (repetitions.Count() != 0)
            {
                return new SResponse(false, "Pole Nazwa musi być unikalne");
            }

            return new SResponse(true, "");
        }
    }
}