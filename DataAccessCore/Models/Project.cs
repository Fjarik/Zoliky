using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Project
    {
        public Project()
        {
            Changelogs = new HashSet<Changelog>();
            Crashes = new HashSet<Crash>();
            News = new HashSet<News>();
            Notifications = new HashSet<Notification>();
            Unavailabilities = new HashSet<Unavailability>();
            Urls = new HashSet<Url>();
            UserLogins = new HashSet<UserLogin>();
            UserSettings = new HashSet<UserSetting>();
            WebEvents = new HashSet<WebEvent>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public string Description { get; set; }
        [StringLength(15)]
        public string Version { get; set; }
        [Required]
        public bool? Active { get; set; }

        [InverseProperty("Project")]
        public virtual ICollection<Changelog> Changelogs { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<Crash> Crashes { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<News> News { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<Notification> Notifications { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<Unavailability> Unavailabilities { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<Url> Urls { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<UserLogin> UserLogins { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<UserSetting> UserSettings { get; set; }
        [InverseProperty("FromProject")]
        public virtual ICollection<WebEvent> WebEvents { get; set; }
    }
}