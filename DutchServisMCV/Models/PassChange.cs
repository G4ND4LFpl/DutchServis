using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    public class PassChange
    {
        [DataType(DataType.Password)]
        public string CurrentPass { get; set; }
        public string NewPass { get; set; }
        public string RepeatPass { get; set; }
    }
}