using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class UserLogin
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("UserID")]
        public int UserID { get; set; }
        [Column("ProjectID")]
        public int ProjectID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public int Status { get; set; }
        [Column("IP")]
        [StringLength(50)]
        public string Ip { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("UserLogins")]
        public virtual Project Project { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("UserLogins")]
        public virtual User User { get; set; }
    }
}