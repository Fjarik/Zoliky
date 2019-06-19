using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Rank
    {
        [Column("ID")]
        public int ID { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        [Column("FromXP")]
        public int FromXp { get; set; }
        [Column("ToXP")]
        public int? ToXp { get; set; }
        [StringLength(20)]
        public string Colour { get; set; }
    }
}