using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    public class Match
    {
        public int Id { get; set; }
        public string Player1 { get; set; }
        public int PointsPlayer1 { get; set; }
        public string Player2 { get; set; }
        public int PointsPlayer2 { get; set; }
        public string Tournament { get; set; }
        public DateTime PlayDate { get; set; }
        public int FormatBo { get; set; }
    }
}