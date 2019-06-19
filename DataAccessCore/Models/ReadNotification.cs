using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class ReadNotification
    {
        [Column("UserID")]
        public int UserID { get; set; }
        [Column("NotificationID")]
        public int NotificationID { get; set; }

        [ForeignKey("NotificationId")]
        [InverseProperty("ReadNotifications")]
        public virtual Notification Notification { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("ReadNotifications")]
        public virtual User User { get; set; }
    }
}