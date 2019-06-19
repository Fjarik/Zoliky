using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class AchievementUnlock
    {
        public int UserID { get; set; }
        public int AchievementID { get; set; }

        [ForeignKey("AchievementId")]
        [InverseProperty("AchievementUnlocks")]
        public virtual Achievement Achievement { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("AchievementUnlocks")]
        public virtual User User { get; set; }
    }
}