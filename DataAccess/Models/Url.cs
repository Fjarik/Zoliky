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
    
    public partial class Url
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int PathID { get; set; }
        public string Name { get; set; }
        public string New { get; set; }
        public bool Enabled { get; set; }
    
        public virtual Path OriginalPath { get; set; }
    }
}