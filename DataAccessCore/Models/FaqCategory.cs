using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class FaqCategory
    {
        public FaqCategory()
        {
            FaQS = new HashSet<FaQ>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        public bool IsFirst { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<FaQ> FaQS { get; set; }
    }
}