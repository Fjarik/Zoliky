using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Post
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("AuthorID")]
        public int AuthorID { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey("AuthorId")]
        [InverseProperty("Posts")]
        public virtual User Author { get; set; }
    }
}