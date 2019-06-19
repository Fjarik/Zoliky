using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Token
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("UserID")]
        public int UserID { get; set; }
        public Guid Code { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Issue { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Expiration { get; set; }
        public int Type { get; set; }
        [Required]
        [StringLength(50)]
        public string Purpose { get; set; }
        public bool Used { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Tokens")]
        public virtual User User { get; set; }
    }
}