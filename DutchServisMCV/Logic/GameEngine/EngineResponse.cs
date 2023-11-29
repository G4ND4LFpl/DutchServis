using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Logic.GameEngine
{
    public struct EngineResponse
    {
        public bool RefreshAll { get; set; }
        public bool RefreshSingle { get; set; }
        public bool RefreshStack { get; set; }
        public bool RefreshDeck { get; set; }
        public bool RefreshButton { get; set; }
        public string[] Args { get; set; }
        public Dictionary<string, Slot> State { get; set; }
    }
}