using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class WebEvent
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("FromProjectID")]
        public int FromProjectID { get; set; }
        [Column("FromID")]
        public int? FromID { get; set; }
        [Column("ToID")]
        public int ToID { get; set; }
        public int Type { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public bool Enabled { get; set; }
        [StringLength(1000)]
        public string Message { get; set; }

        [ForeignKey("FromId")]
        [InverseProperty("WebEventFroms")]
        public virtual User From { get; set; }
        [ForeignKey("FromProjectId")]
        [InverseProperty("WebEvents")]
        public virtual Project FromProject { get; set; }
        [ForeignKey("ToId")]
        [InverseProperty("WebEventTos")]
        public virtual User To { get; set; }
    }
}