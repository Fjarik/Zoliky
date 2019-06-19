using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class TicketComment
    {
        [Column("ID")]
        public int ID { get; set; }
        public int TicketID { get; set; }
        public int UserID { get; set; }
        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("TicketId")]
        [InverseProperty("TicketComments")]
        public virtual Ticket Ticket { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("TicketComments")]
        public virtual User User { get; set; }
    }
}