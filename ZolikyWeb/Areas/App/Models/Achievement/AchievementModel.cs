using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;

namespace ZolikyWeb.Areas.App.Models.Achievement
{
	public class AchievementModel : DataAccess.Models.Achievement
	{
		public bool IsUnlocked { get; set; } = false;
		public DateTime? When { get; set; } = null;

		public string CardStyle => IsUnlocked ? "" : "background-color: #e6e6e6;";
		public string ImageStyle => IsUnlocked ? "" : "filter: grayscale(100%);";

		public AchievementModel() { }

		public AchievementModel(IAchievement ach)
		{
			this.ID = ach.ID;
			this.Title = ach.Title;
			this.Description = ach.Description;
			this.XP = ach.XP;
			this.Enabled = ach.Enabled;
			this.UnlockedImageID = ach.UnlockedImageID;
			this.ImageLockedID = ach.ImageLockedID;
		}
	}
}