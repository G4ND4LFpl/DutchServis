namespace DutchServisMCV.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity.Infrastructure;
    using DutchServisMCV.Models.GameNamespace;

    /// <include file='Documentation.xml' path='dock/class[@name="PlayerData"]/PlayerData/*'/>
    public class PlayerData : PlayerInfo
    {
        /// <include file='Documentation.xml' path='dock/class[@name="PlayerData"]/Name/*'/>
        public string Name { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="PlayerData"]/Surname/*'/>
        public string Surname { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="PlayerData"]/Id/*'/>
        public int Id { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="PlayerData"]/JoinDate/*'/>
        public DateTime? JoinDate { get; set; }
        /// <include file='Documentation.xml' path='dock/class[@name="PlayerData"]/Status/*'/>
        public string Status { get; set; }

        public Statistics Stats { get; set; }
    }

   
}