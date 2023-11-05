namespace DutchServisMCV.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity.Infrastructure;

    /// <include file='Documentation.xml' path='dock/class[@name="PlayerInfo"]/PlayerInfo/*'/>
    public partial class PlayerInfo
    {
        /// <include file='Documentation.xml' path='dock/class[@name="PlayerInfo"]/Nickname/*'/>
        public string Nickname { get; set; }

        /// <include file='Documentation.xml' path='dock/class[@name="PlayerInfo"]/Img/*'/>
        public string Img { get; set; }

        /// <include file='Documentation.xml' path='dock/class[@name="PlayerInfo"]/Clan/*'/>
        public string Clan { get; set; }

        /// <include file='Documentation.xml' path='dock/class[@name="PlayerInfo"]/Ranking/*'/>
        public Nullable<double> Ranking { get; set; }

        /// <include file='Documentation.xml' path='dock/class[@name="PlayerInfo"]/Rating/*'/>
        public Nullable<double> Rating { get; set; }

        /// <include file='Documentation.xml' path='dock/class[@name="PlayerInfo"]/Active/*'/>
        public bool? Active { get; set; }
    }
}