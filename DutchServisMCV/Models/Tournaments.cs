//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DutchServisMCV.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tournaments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tournaments()
        {
            this.ShowResults = false;
        }
    
        public int TournamentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Location { get; set; }
        public string Theme { get; set; }
        public string Info { get; set; }
        public string Img { get; set; }
        public Nullable<bool> ShowResults { get; set; }
    }
}
