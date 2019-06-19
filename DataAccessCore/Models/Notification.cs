using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Notification
    {
        public Notification()
        {
            ReadNotifications = new HashSet<ReadNotification>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [Column("ToID")]
        public int? ToID { get; set; }
        [Column("FromID")]
        public int? FromID { get; set; }
        [Column("ProjectID")]
        public int? ProjectID { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Expiration { get; set; }
        public bool Visible { get; set; }

        [ForeignKey("FromId")]
        [InverseProperty("NotificationFroms")]
        public virtual User From { get; set; }
        [ForeignKey("ProjectId")]
        [InverseProperty("Notifications")]
        public virtual Project Project { get; set; }
        [ForeignKey("ToId")]
        [InverseProperty("NotificationTos")]
        public virtual User To { get; set; }
        [InverseProperty("Notification")]
        public virtual ICollection<ReadNotification> ReadNotifications { get; set; }
    }
}