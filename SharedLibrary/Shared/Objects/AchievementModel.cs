using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Shared.Objects
{
	public class AchievementModel : IAchievement
	{
#region Entity

		public int ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int XP { get; set; }
		public bool Enabled { get; set; }

#endregion

		public bool IsUnlocked { get; set; } = false;
		public bool IsNotUnlocked => !this.IsUnlocked;
		public DateTime? When { get; set; } = null;

		public AchievementModel() { }

		public AchievementModel(IAchievement ach)
		{
			this.ID = ach.ID;
			this.Title = ach.Title;
			this.Description = ach.Description;
			this.XP = ach.XP;
			this.Enabled = ach.Enabled;
		}
	}
}