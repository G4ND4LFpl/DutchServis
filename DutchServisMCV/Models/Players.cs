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
    using System.Web;

    public partial class Players
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Players()
        {
            this.Rating = 1000D;
            this.JoinDate = DateTime.Now;
            this.Active = true;
        }
    
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Nickname { get; set; }
        public Nullable<int> ClanId { get; set; }
        public Nullable<double> Rating { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public string Img { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}
