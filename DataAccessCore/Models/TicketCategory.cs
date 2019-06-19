using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class TicketCategory
    {
        public TicketCategory()
        {
            AnonymTickets = new HashSet<AnonymTicket>();
            Tickets = new HashSet<Ticket>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<AnonymTicket> AnonymTickets { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}