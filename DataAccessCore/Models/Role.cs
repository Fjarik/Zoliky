using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}