using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    namespace GameNamespace
    {
        public class PlayerItem
        {
            public int Id { get; set; }
            public string Nickname { get; set; }
        }

        public class PlayerTournItem : PlayerItem
        {
            public Nullable<double> RankingBefore { get; set; }
            public string Place { get; set; }
            public Nullable<double> RankingGet { get; set; }
            public Nullable<double> Price { get; set; }
        }

        public class PlayerLeagueItem : PlayerItem
        {
            public int Points { get; set; }
            public int Won { get; set; }
            public int Loose { get; set; }
            public int Draw { get; set; }
            public Nullable<double> Price { get; set; }
        }

        public class GamesSum
        {
            public int MatchId { get; set; }
            public string Nickname { get; set; }
            public int Points { get; set; }
        }

        public class Stat
        {
            public double? Percentage { get; set; }
            public int Win { get; set; }
            public int Total { get; set; }
        }

        public class Statistics
        {
            public Stat Games { get; set; }
            public Stat Openings { get; set; }
            public Stat Dutches { get; set; }

            public double? AvgPoints { get; set; }
            public double? AvgPointsWin { get; set; }
            public double? AvgPointsLoose { get; set; }
            public int ClearBoards { get; set; }
        }
    }
}