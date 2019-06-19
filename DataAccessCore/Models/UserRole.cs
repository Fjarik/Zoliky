using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class UserRole
    {
        [Column("UserID")]
        public int UserID { get; set; }
        [Column("RoleID")]
        public int RoleID { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("UserRoles")]
        public virtual Role Role { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("UserRoles")]
        public virtual User User { get; set; }
    }
}