using System;
using System.Collections.Generic;
using DutchServisMCV.Models.GameNamespace;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Web;

namespace DutchServisMCV.Models
{
    public class LeagueInfo : CompetitionInfo<PlayerLeagueItem>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public static int CompareByRankingGet(PlayerLeagueItem p1, PlayerLeagueItem p2)
        {
            if (p1.Points > p2.Points) return 1;
            else if (p1.Points == p2.Points) return 0;
            else return -1;
        }
    }
}