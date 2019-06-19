using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Url
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("ProjectID")]
        public int ProjectID { get; set; }
        [Column("PathID")]
        public int PathID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(200)]
        public string New { get; set; }
        [Required]
        public bool? Enabled { get; set; }

        [ForeignKey("PathId")]
        [InverseProperty("Urls")]
        public virtual Path Path { get; set; }
        [ForeignKey("ProjectId")]
        [InverseProperty("Urls")]
        public virtual Project Project { get; set; }
    }
}