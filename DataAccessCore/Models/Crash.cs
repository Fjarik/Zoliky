using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Crash
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("ProjectID")]
        public int ProjectID { get; set; }
        [Column("ScreenshotID")]
        public int? ScreenshotID { get; set; }
        [Column("UserID")]
        public int? UserID { get; set; }
        public byte Status { get; set; }
        [StringLength(50)]
        public string AppVersion { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Build { get; set; }
        [StringLength(200)]
        public string LastWindowName { get; set; }
        [Column("OS")]
        [StringLength(200)]
        public string Os { get; set; }
        [Column("Is64BitOS")]
        public bool? Is64BitOs { get; set; }
        public bool? Is64BitApp { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public string Exception { get; set; }
        [StringLength(2000)]
        public string Message { get; set; }
        [StringLength(250)]
        public string Email { get; set; }
        public string Log { get; set; }
        [Required]
        public bool? Enabled { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("Crashes")]
        public virtual Project Project { get; set; }
        [ForeignKey("ScreenshotId")]
        [InverseProperty("Crashes")]
        public virtual Image Screenshot { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Crashes")]
        public virtual User User { get; set; }
    }
}