using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Transaction
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("FromID")]
        public int FromID { get; set; }
        [Column("ToID")]
        public int ToID { get; set; }
        [Column("ZolikID")]
        public int ZolikID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        [StringLength(400)]
        public string Message { get; set; }
        public byte Typ { get; set; }

        [ForeignKey("FromId")]
        [InverseProperty("TransactionFroms")]
        public virtual User From { get; set; }
        [ForeignKey("ToId")]
        [InverseProperty("TransactionTos")]
        public virtual User To { get; set; }
        [ForeignKey("ZolikId")]
        [InverseProperty("Transactions")]
        public virtual Zolik Zolik { get; set; }
    }
}