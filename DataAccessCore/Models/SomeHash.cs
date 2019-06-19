using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class SomeHash
    {
        public SomeHash()
        {
            Users = new HashSet<User>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [Column("OwnerID")]
        public int OwnerID { get; set; }
        [MaxLength(32)]
        public byte[] Salt { get; set; }
        [Required]
        [StringLength(100)]
        public string Hash { get; set; }
        [Required]
        [StringLength(20)]
        public string Version { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Expiration { get; set; }

        [ForeignKey("OwnerId")]
        [InverseProperty("SomeHashes")]
        public virtual User Owner { get; set; }
        [InverseProperty("Password")]
        public virtual ICollection<User> Users { get; set; }
    }
}