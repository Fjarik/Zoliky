using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class AnonymTicketComment
    {
        [Column("ID")]
        public int ID { get; set; }
        public int TicketID { get; set; }
        public int? UserID { get; set; }
        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [StringLength(50)]
        public string Ip { get; set; }

        [ForeignKey("TicketId")]
        [InverseProperty("AnonymTicketComments")]
        public virtual AnonymTicket Ticket { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("AnonymTicketComments")]
        public virtual User User { get; set; }
    }
}