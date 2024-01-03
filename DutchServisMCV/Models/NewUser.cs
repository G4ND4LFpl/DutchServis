using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    public class NewUser
    {
        public string Username { get; set; }
        public string Pass { get; set; }
        public string RepeatPass { get; set; }
    }
}