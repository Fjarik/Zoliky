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
    
    public partial class Achievement
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int XP { get; set; }
        public bool Enabled { get; set; }
        public Nullable<int> ValueToUnlock { get; set; }
        public string RelatedKey { get; set; }
    }
}
