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
    
    public partial class Notification
    {
        public int ID { get; set; }
        public int ToID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<System.DateTime> Expiration { get; set; }
        public bool Seen { get; set; }
        public bool Removed { get; set; }
        public SharedLibrary.Enums.NotificationSeverity Severity { get; set; }
        public string Icon { get; set; }
    }
}
