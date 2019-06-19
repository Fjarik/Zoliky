using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class UserSetting
    {
        [Column("ID")]
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        [StringLength(50)]
        public string Key { get; set; }
        [StringLength(500)]
        public string Value { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Changed { get; set; }

        [ForeignKey("Id")]
        [InverseProperty("UserSettings")]
        public virtual User IdNavigation { get; set; }
        [ForeignKey("ProjectId")]
        [InverseProperty("UserSettings")]
        public virtual Project Project { get; set; }
    }
}