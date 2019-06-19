using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Unavailability
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("ProjectID")]
        public int? ProjectID { get; set; }
        [StringLength(50)]
        public string Reason { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime From { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime To { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("Unavailabilities")]
        public virtual Project Project { get; set; }
    }
}