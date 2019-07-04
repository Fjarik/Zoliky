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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.UserSettings = new HashSet<UserSetting>();
            this.Roles = new HashSet<Role>();
            this.ReadNotifications = new HashSet<Notification>();
            this.Achievements = new HashSet<Achievement>();
            this.OriginalZoliks = new HashSet<Zolik>();
            this.LoginTokens = new HashSet<UserLoginToken>();
            this.Logins = new HashSet<UserLogin>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ProfilePhotoID { get; set; }
        public Nullable<int> ClassID { get; set; }
        public Nullable<int> PasswordID { get; set; }
        public System.Guid UQID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public SharedLibrary.Enums.Sex Sex { get; set; }
        public SharedLibrary.Enums.UserPermission Type { get; set; }
        public bool Enabled { get; set; }
        public System.DateTime MemberSince { get; set; }
        public string RegistrationIp { get; set; }
        public string Description { get; set; }
        public string VersionS { get; set; }
        public int XP { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual SomeHash Password { get; set; }
        public virtual Image ProfilePhoto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSetting> UserSettings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Role> Roles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> ReadNotifications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Achievement> Achievements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Zolik> OriginalZoliks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLoginToken> LoginTokens { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        private ICollection<UserLogin> Logins { get; set; }
    }
}
