using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Term
    {
        public Term()
        {
            Consents = new HashSet<Consent>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [StringLength(200)]
        public string Title { get; set; }
        public string Head { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        [StringLength(10)]
        public string Shortcut { get; set; }
        [Column(TypeName = "date")]
        public DateTime Since { get; set; }
        [Column(TypeName = "date")]
        public DateTime? To { get; set; }

        [InverseProperty("Term")]
        public virtual ICollection<Consent> Consents { get; set; }
    }
}