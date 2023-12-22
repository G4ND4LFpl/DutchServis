using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Logic.GameEngine
{
    public struct EngineResponse
    {
        public int Round { get; set; }
        public bool RefreshAllCards { get; set; }
        public bool RefreshSingleCard { get; set; }
        public Dictionary<string, bool> Refresh { get; set; }
        public string[] Args { get; set; }
        public Dictionary<string, Slot> State { get; set; }
    }
}