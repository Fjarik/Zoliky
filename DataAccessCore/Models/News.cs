using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class News
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("ProjectID")]
        public int? ProjectID { get; set; }
        [StringLength(13)]
        public string Title { get; set; }
        public string Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        public bool LoginOnly { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Expiration { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("News")]
        public virtual Project Project { get; set; }
    }
}