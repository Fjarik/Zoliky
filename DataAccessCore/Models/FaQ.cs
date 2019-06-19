using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    [Table("FaQ")]
    public partial class FaQ
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("CategoryID")]
        public int CategoryID { get; set; }
        [Required]
        [StringLength(100)]
        public string Question { get; set; }
        [Required]
        public string Response { get; set; }
        public bool IsFirst { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("FaQS")]
        public virtual FaqCategory Category { get; set; }
    }
}