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
    
    public partial class GetTopStudentsXp_Result
    {
        public int ID { get; set; }
        public Nullable<int> ClassID { get; set; }
        public int SchoolID { get; set; }
        public Nullable<int> ProfilePhotoID { get; set; }
        public string ClassName { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public int XP { get; set; }
        public string Hash { get; set; }
        public string Base64 { get; set; }
        public string MIME { get; set; }
        public Nullable<int> Size { get; set; }
        public Nullable<int> ImageID { get; set; }
    }
}