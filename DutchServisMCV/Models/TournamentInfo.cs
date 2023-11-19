using System;
using System.Collections.Generic;
using DutchServisMCV.Models.GameNamespace;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Web;

namespace DutchServisMCV.Models
{
    public class TournamentInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public string Location { get; set; }
        public string Theme { get; set; }
        public string Info { get; set; }
        public string Img { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<MatchData> Matches { get; set; }
        public List<PlayerTournItem> Players { get; set; }
        public bool ShowResults { get; set; }

        public static int CompareByRankingBefore(PlayerTournItem p1, PlayerTournItem p2)
        {
            if (p1.RankingBefore > p2.RankingBefore) return 1;
            else if (p1.RankingBefore == p2.RankingBefore) return 0;
            else return -1;
        }
        public static int CompareByRankingGet(PlayerTournItem p1, PlayerTournItem p2)
        {
            if (p1.RankingGet > p2.RankingGet) return 1;
            else if (p1.RankingGet == p2.RankingGet) return 0;
            else return -1;
        }
    }
}