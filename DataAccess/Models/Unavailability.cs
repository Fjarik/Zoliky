//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Unavailability
    {
        public int ID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public System.DateTime From { get; set; }
        public System.DateTime To { get; set; }
    }
}
