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
    
    public partial class TeacherSubject
    {
        public int TeacherID { get; set; }
        public int SubjectID { get; set; }
        public int ClassID { get; set; }
        public System.DateTime Created { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual User Teacher { get; set; }
    }
}
