using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedLibrary.Enums;

namespace DataAccessCore.Models
{
    [Table("Zoliky")]
    public partial class Zolik
    {
        public Zolik()
        {
            Transactions = new HashSet<Transaction>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [Column("TeacherID")]
        public int TeacherID { get; set; }
        [Column("SubjectID")]
        public int SubjectID { get; set; }
        [Column("OwnerID")]
        public int OwnerID { get; set; }
        [Column("OriginalOwnerID")]
        public int OriginalOwnerID { get; set; }
        public ZolikType Type { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OwnerSince { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        public bool Enabled { get; set; }
        [StringLength(50)]
        public string Lock { get; set; }
        public bool AllowSplit { get; set; }

        [ForeignKey("OriginalOwnerId")]
        [InverseProperty("ZolikyOriginalOwners")]
        public virtual User OriginalOwner { get; set; }
        [ForeignKey("OwnerId")]
        [InverseProperty("ZolikyOwners")]
        public virtual User Owner { get; set; }
        [ForeignKey("SubjectId")]
        [InverseProperty("Zolikies")]
        public virtual Subject Subject { get; set; }
        [ForeignKey("TeacherId")]
        [InverseProperty("ZolikyTeachers")]
        public virtual User Teacher { get; set; }
        [InverseProperty("Zolik")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}