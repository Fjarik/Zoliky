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
    
    public partial class Term
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Head { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        public string Shortcut { get; set; }
        public System.DateTime Since { get; set; }
        public Nullable<System.DateTime> To { get; set; }
    }
}