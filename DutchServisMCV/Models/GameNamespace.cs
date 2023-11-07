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
            public double RankingBefore { get; set; }
            public string Place { get; set; }
            public double RankingGet { get; set; }
            public Nullable<double> Price { get; set; }

            public static int CompareByRankingBefore(PlayerItem p1, PlayerItem p2)
            {
                if (p1.RankingBefore > p2.RankingBefore) return 1;
                else if (p1.RankingBefore == p2.RankingBefore) return 0;
                else return -1;
            }
            public static int CompareByRankingGet(PlayerItem p1, PlayerItem p2)
            {
                if (p1.RankingGet > p2.RankingGet) return 1;
                else if (p1.RankingGet == p2.RankingGet) return 0;
                else return -1;
            }
        }

        public class GamesSum
        {
            public int MatchId { get; set; }
            public string Nickname { get; set; }
            public int Points { get; set; }
        }
    }
}