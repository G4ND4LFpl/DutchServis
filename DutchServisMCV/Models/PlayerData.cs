namespace DutchServisMCV.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity.Infrastructure;

    public partial class PlayerData
    {
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime JoinDate { get; set; } 
        public string Img { get; set; }
        public string Clan { get; set; }
        public double? Ranking { get; set; }
        public double? Rating { get; set; }
        public double? Winratio { get; set; }
        public int WinGames { get; set; }
        public int TotalGames { get; set; }
        public double? OpenWinration { get; set; }
        public int WinOpenings { get; set; }
        public int Openings { get; set; }
        public double? DutchWinration { get; set; }
        public int WinDutchs { get; set; }
        public int Dutchs { get; set; }
        public string Status { get; set; }
    }
}