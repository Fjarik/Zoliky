using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class UserLoginToken
    {
        [Column("UserID")]
        public int UserID { get; set; }
        [StringLength(128)]
        public string LoginProvider { get; set; }
        [StringLength(128)]
        public string ProviderKey { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("UserLoginTokens")]
        public virtual User User { get; set; }
    }
}