using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessCore.Models
{
	public partial class Achievement
	{
		public Achievement()
		{
			AchievementUnlocks = new HashSet<AchievementUnlock>();
		}

		[Column("ID")]
		public int ID { get; set; }

		[Column("UnlockedImageID")]
		public int? UnlockedImageID { get; set; }

		[Column("ImageLockedID")]
		public int? ImageLockedID { get; set; }

		[StringLength(100)]
		public string Title { get; set; }

		[StringLength(100)]
		public string Description { get; set; }

		[Column("XP")]
		public int Xp { get; set; }

		[Required]
		public bool? Enabled { get; set; }

		[ForeignKey("ImageLockedId")]
		[InverseProperty("AchievementImageLockeds")]
		public virtual Image ImageLocked { get; set; }

		[ForeignKey("UnlockedImageId")]
		[InverseProperty("AchievementUnlockedImages")]
		public virtual Image UnlockedImage { get; set; }

		[InverseProperty("Achievement")]
		public virtual ICollection<AchievementUnlock> AchievementUnlocks { get; set; }
	}
}