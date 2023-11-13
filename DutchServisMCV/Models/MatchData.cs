using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    public class MatchData
    {
        public int Id { get; set; }
        public string Tournament { get; set; }
        public string Player1 { get; set; }
        public Nullable<int> PointsPlayer1 { get; set; }
        public string Player2 { get; set; }
        public Nullable<int> PointsPlayer2 { get; set; }
        public DateTime PlayDate { get; set; }
        public Nullable<bool> BonusGamePlayer1 { get; set; }
        public Nullable<bool> BonusGamePlayer2 { get; set; }
        public int FormatBo { get; set; }
        public List<Games> Games { get; set; }
    }
}