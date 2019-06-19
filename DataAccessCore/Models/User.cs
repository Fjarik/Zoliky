using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class User
    {
        public User()
        {
            AchievementUnlocks = new HashSet<AchievementUnlock>();
            AnonymTicketComments = new HashSet<AnonymTicketComment>();
            AnonymTickets = new HashSet<AnonymTicket>();
            Consents = new HashSet<Consent>();
            Crashes = new HashSet<Crash>();
            Images = new HashSet<Image>();
            NotificationFroms = new HashSet<Notification>();
            NotificationTos = new HashSet<Notification>();
            Posts = new HashSet<Post>();
            ReadNotifications = new HashSet<ReadNotification>();
            SomeHashes = new HashSet<SomeHash>();
            TicketAdmins = new HashSet<Ticket>();
            TicketComments = new HashSet<TicketComment>();
            TicketUsers = new HashSet<Ticket>();
            Tokens = new HashSet<Token>();
            TransactionFroms = new HashSet<Transaction>();
            TransactionTos = new HashSet<Transaction>();
            UserLoginTokens = new HashSet<UserLoginToken>();
            UserLogins = new HashSet<UserLogin>();
            UserRoles = new HashSet<UserRole>();
            UserSettings = new HashSet<UserSetting>();
            WebEventFroms = new HashSet<WebEvent>();
            WebEventTos = new HashSet<WebEvent>();
            ZolikyOriginalOwners = new HashSet<Zolik>();
            ZolikyOwners = new HashSet<Zolik>();
            ZolikyTeachers = new HashSet<Zolik>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [Column("ProfilePhotoID")]
        public int? ProfilePhotoID { get; set; }
        [Column("ClassID")]
        public int? ClassID { get; set; }
        [Column("PasswordID")]
        public int? PasswordID { get; set; }
        [Column("UQID")]
        public Guid Uqid { get; set; }
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        [Required]
        [StringLength(300)]
        public string Email { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Lastname { get; set; }
        public byte Sex { get; set; }
        public byte Type { get; set; }
        public bool Enabled { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime MemberSince { get; set; }
        [Column("RegistrationIP")]
        [StringLength(100)]
        public string RegistrationIp { get; set; }
        public string Description { get; set; }
        [StringLength(8)]
        public string Version { get; set; }
        [Column("XP")]
        public int Xp { get; set; }

        [ForeignKey("ClassId")]
        [InverseProperty("Users")]
        public virtual Class Class { get; set; }
        [ForeignKey("PasswordId")]
        [InverseProperty("Users")]
        public virtual SomeHash Password { get; set; }
        [ForeignKey("ProfilePhotoId")]
        [InverseProperty("Users")]
        public virtual Image ProfilePhoto { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<AchievementUnlock> AchievementUnlocks { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<AnonymTicketComment> AnonymTicketComments { get; set; }
        [InverseProperty("Admin")]
        public virtual ICollection<AnonymTicket> AnonymTickets { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Consent> Consents { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Crash> Crashes { get; set; }
        [InverseProperty("Owner")]
        public virtual ICollection<Image> Images { get; set; }
        [InverseProperty("From")]
        public virtual ICollection<Notification> NotificationFroms { get; set; }
        [InverseProperty("To")]
        public virtual ICollection<Notification> NotificationTos { get; set; }
        [InverseProperty("Author")]
        public virtual ICollection<Post> Posts { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<ReadNotification> ReadNotifications { get; set; }
        [InverseProperty("Owner")]
        public virtual ICollection<SomeHash> SomeHashes { get; set; }
        [InverseProperty("Admin")]
        public virtual ICollection<Ticket> TicketAdmins { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Ticket> TicketUsers { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Token> Tokens { get; set; }
        [InverseProperty("From")]
        public virtual ICollection<Transaction> TransactionFroms { get; set; }
        [InverseProperty("To")]
        public virtual ICollection<Transaction> TransactionTos { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserLoginToken> UserLoginTokens { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserLogin> UserLogins { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserRole> UserRoles { get; set; }
        [InverseProperty("IdNavigation")]
        public virtual ICollection<UserSetting> UserSettings { get; set; }
        [InverseProperty("From")]
        public virtual ICollection<WebEvent> WebEventFroms { get; set; }
        [InverseProperty("To")]
        public virtual ICollection<WebEvent> WebEventTos { get; set; }
        [InverseProperty("OriginalOwner")]
        public virtual ICollection<Zolik> ZolikyOriginalOwners { get; set; }
        [InverseProperty("Owner")]
        public virtual ICollection<Zolik> ZolikyOwners { get; set; }
        [InverseProperty("Teacher")]
        public virtual ICollection<Zolik> ZolikyTeachers { get; set; }
    }
}