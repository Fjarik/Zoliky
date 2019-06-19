using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class AnonymTicket
    {
        public AnonymTicket()
        {
            AnonymTicketComments = new HashSet<AnonymTicketComment>();
        }

        [Column("ID")]
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int? AdminID { get; set; }
        public int Status { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        public int Code { get; set; }
        [StringLength(50)]
        public string Ip { get; set; }

        [ForeignKey("AdminId")]
        [InverseProperty("AnonymTickets")]
        public virtual User Admin { get; set; }
        [ForeignKey("CategoryId")]
        [InverseProperty("AnonymTickets")]
        public virtual TicketCategory Category { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<AnonymTicketComment> AnonymTicketComments { get; set; }
    }
}