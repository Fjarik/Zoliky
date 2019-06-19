using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Price
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public int ZolikType { get; set; }
    }
}