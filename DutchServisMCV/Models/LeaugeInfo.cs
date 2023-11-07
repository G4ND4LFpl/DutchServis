using System;
using System.Collections.Generic;
using DutchServisMCV.Models.GameNamespace;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace DutchServisMCV.Models
{
    public class LeaugeInfo
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Info { get; set; }
        public string Img { get; set; }
        public List<Match> Matches { get; set; }
        public List<PlayerItem> Players { get; set; }
    }
}