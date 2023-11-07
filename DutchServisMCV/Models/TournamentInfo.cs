﻿using System;
using System.Collections.Generic;
using DutchServisMCV.Models.GameNamespace;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace DutchServisMCV.Models
{
    public class TournamentInfo
    {
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public string Theme { get; set; }
        public string Info { get; set; }
        public string Img { get; set; }
        public List<Match> Matches { get; set; }
        public List<PlayerItem> Players { get; set; }
    }
}