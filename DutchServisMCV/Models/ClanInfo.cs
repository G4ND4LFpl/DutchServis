using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    public class ClanInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public List<PlayerInfo> Players { get; set; }
    }
}