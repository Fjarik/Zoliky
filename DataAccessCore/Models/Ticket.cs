using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Ticket
    {
        public Ticket()
        {
            TicketComments = new HashSet<TicketComment>();
        }

        [Column("ID")]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public int? AdminID { get; set; }
        public int Status { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("AdminId")]
        [InverseProperty("TicketAdmins")]
        public virtual User Admin { get; set; }
        [ForeignKey("CategoryId")]
        [InverseProperty("Tickets")]
        public virtual TicketCategory Category { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("TicketUsers")]
        public virtual User User { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<TicketComment> TicketComments { get; set; }
    }
}