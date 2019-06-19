using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Class
    {
        public Class()
        {
            Users = new HashSet<User>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [StringLength(3)]
        public string Name { get; set; }
        [Column(TypeName = "date")]
        public DateTime Since { get; set; }
        [Column(TypeName = "date")]
        public DateTime Graduation { get; set; }
        [Required]
        public bool? Enabled { get; set; }

        [InverseProperty("Class")]
        public virtual ICollection<User> Users { get; set; }
    }
}