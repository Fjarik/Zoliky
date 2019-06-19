using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Changelog
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("ProjectID")]
        public int ProjectID { get; set; }
        [StringLength(200)]
        public string Title { get; set; }
        public string Text { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        [StringLength(10)]
        public string Version { get; set; }
        public bool Visible { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("Changelogs")]
        public virtual Project Project { get; set; }
    }
}