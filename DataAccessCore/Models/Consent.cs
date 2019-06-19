using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Consent
    {
        [Column("UserID")]
        public int UserID { get; set; }
        [Column("TermID")]
        public int TermID { get; set; }
        public bool Accepted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime When { get; set; }

        [ForeignKey("TermId")]
        [InverseProperty("Consents")]
        public virtual Term Term { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Consents")]
        public virtual User User { get; set; }
    }
}