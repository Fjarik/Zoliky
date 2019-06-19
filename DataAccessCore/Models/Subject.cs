using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Subject
    {
        public Subject()
        {
            Zolikies = new HashSet<Zolik>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [Required]
        [StringLength(5)]
        public string Shortcut { get; set; }

        [InverseProperty("Subject")]
        public virtual ICollection<Zolik> Zolikies { get; set; }
    }
}