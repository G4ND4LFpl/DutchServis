using DutchServisMCV.Models.GameNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    public class CompetitionInfo<T>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Img { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<MatchInfo> Matches { get; set; }
        public List<T> Players { get; set; }
        public bool ShowResults { get; set; }
    }
}