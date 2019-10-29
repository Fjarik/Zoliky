using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared.Objects;

namespace ZolikyWeb.Areas.App.Models.Achievement
{
	public class AchievementMvcModel : AchievementModel
	{
		public string CardStyle => IsUnlocked ? "" : "background-color: #e6e6e6;";
		public string ImageStyle => IsUnlocked ? "" : "filter: grayscale(100%);";

		public AchievementMvcModel() { }

		public AchievementMvcModel(IAchievement ach) : base(ach) { }
	}
}