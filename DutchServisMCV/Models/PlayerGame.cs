using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    public class PlayerGame
    {
        public string Nickname { get; set; }
        public int Points { get; set; }
        public int Mistakes { get; set; }
        public bool Win { get; set; }
        public bool Open { get; set; }
        public bool Dutch { get; set; }
        public bool Clean { get; set; }
    }
}