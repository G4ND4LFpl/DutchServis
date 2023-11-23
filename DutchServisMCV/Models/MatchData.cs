using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    public class MatchData : MatchInfo
    {
        public int TournamentId { get; set; }
        public int PlayerId1 { get; set; }
        public int PlayerId2 { get; set; }
        public List<Games> Games { get; set; }
    }
}