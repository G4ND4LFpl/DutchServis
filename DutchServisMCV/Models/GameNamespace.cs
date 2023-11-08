using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    namespace GameNamespace
    {
        public class Match
        {
            public int Id { get; set; }
            public string Player1 { get; set; }
            public Nullable<int> PointsPlayer1 { get; set; }
            public string Player2 { get; set; }
            public Nullable<int> PointsPlayer2 { get; set; }
            public string Tournament { get; set; }
            public DateTime PlayDate { get; set; }
            public int FormatBo { get; set; }
        }
        public class PlayerItem
        {
            public string Nickname { get; set; }
            public Nullable<double> Price { get; set; }
        }

        public class PlayerTournItem : PlayerItem
        {
            public double RankingBefore { get; set; }
            public string Place { get; set; }
            public double RankingGet { get; set; }
        }

        public class PlayerLeagueItem : PlayerItem
        {
            public int Points { get; set; }
            public int Won { get; set; }
            public int Loose { get; set; }
            public int Draw { get; set; }
        }

        public class GamesSum
        {
            public int MatchId { get; set; }
            public string Nickname { get; set; }
            public int Points { get; set; }
        }
    }
}