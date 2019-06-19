using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Path
    {
        public Path()
        {
            Urls = new HashSet<Url>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [StringLength(400)]
        public string Url { get; set; }

        [InverseProperty("Path")]
        public virtual ICollection<Url> Urls { get; set; }
    }
}