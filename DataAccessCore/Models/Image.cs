using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
    public partial class Image
    {
        public Image()
        {
            AchievementImageLockeds = new HashSet<Achievement>();
            AchievementUnlockedImages = new HashSet<Achievement>();
            Crashes = new HashSet<Crash>();
            Users = new HashSet<User>();
        }

        [Column("ID")]
        public int ID { get; set; }
        [Column("OwnerID")]
        public int? OwnerID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Uploaded { get; set; }
        [Column("URL")]
        public string Url { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [StringLength(330)]
        public string Hash { get; set; }
        public string Base64 { get; set; }
        [Column("MIME")]
        [StringLength(20)]
        public string Mime { get; set; }
        public int Size { get; set; }

        [ForeignKey("OwnerId")]
        [InverseProperty("Images")]
        public virtual User Owner { get; set; }
        [InverseProperty("ImageLocked")]
        public virtual ICollection<Achievement> AchievementImageLockeds { get; set; }
        [InverseProperty("UnlockedImage")]
        public virtual ICollection<Achievement> AchievementUnlockedImages { get; set; }
        [InverseProperty("Screenshot")]
        public virtual ICollection<Crash> Crashes { get; set; }
        [InverseProperty("ProfilePhoto")]
        public virtual ICollection<User> Users { get; set; }
    }
}