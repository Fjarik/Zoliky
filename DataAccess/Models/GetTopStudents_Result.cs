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
    
    public partial class GetTopStudents_Result
    {
        public int ID { get; set; }
        public Nullable<int> ClassID { get; set; }
        public Nullable<int> ProfilePhotoID { get; set; }
        public string ClassName { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public int XP { get; set; }
        public Nullable<int> ZolikCount { get; set; }
        private string Hash { get; set; }
        private string Base64 { get; set; }
        private string MIME { get; set; }
        private Nullable<int> Size { get; set; }
        private Nullable<int> ImageID { get; set; }
        public int SchoolID { get; set; }
    }
}
