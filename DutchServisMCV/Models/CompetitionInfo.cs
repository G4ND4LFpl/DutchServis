using DutchServisMCV.Models.GameNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Models
{
    /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/CompetitionInfo/*'/>
    public class CompetitionInfo<T> where T : PlayerItem
    {
        /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/Id/*'/>
        public int Id { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/Name/*'/>
        public string Name { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/Info/*'/>
        public string Info { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/Img/*'/>
        public string Img { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/File/*'/>
        public HttpPostedFileBase File { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/Matches/*'/>
        public List<MatchInfo> Matches { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/Players/*'/>
        public List<T> Players { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="CompetitionInfo"]/ShowResults/*'/>
        public bool ShowResults { get; set; }
    }
}