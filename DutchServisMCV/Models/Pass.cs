using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    /// <include file='Documentation.xml' path='dock/class[@name="Pass"]/Pass/*'/>
    public class Pass
    {
        /// <include file='Documentation.xml' path='dock/class[@name="Pass"]/CurrentPass/*'/>
        [DataType(DataType.Password)]
        public string CurrentPass { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="Pass"]/NewPass/*'/>
        public string NewPass { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="Pass"]/RepeatPass/*'/>
        public string RepeatPass { get; set; }
    }
}