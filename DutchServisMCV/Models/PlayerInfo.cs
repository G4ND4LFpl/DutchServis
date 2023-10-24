namespace DutchServisMCV.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity.Infrastructure;

    public partial class PlayerInfo
    {
        public string Nickname { get; set; }
        public string Img { get; set; }
        public string Clan { get; set; }
        public Nullable<double> Ranking { get; set; }
        public Nullable<double> Rating { get; set; }
    }
}