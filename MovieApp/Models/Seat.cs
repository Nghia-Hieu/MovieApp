//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MovieApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Seat
    {
        public string show_id { get; set; }
        public string seat_id { get; set; }
        public long price { get; set; }
        public bool status { get; set; }
        public string user_id { get; set; }
    
        public virtual ShowTime ShowTime { get; set; }
        public virtual User User { get; set; }
    }
}
