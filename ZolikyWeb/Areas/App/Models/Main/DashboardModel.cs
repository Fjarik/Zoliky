using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess.Models;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;

namespace ZolikyWeb.Areas.App.Models.Main
{
	public class DashboardModel
	{
		public int ZolikCount { get; set; } = 0;

		public int JokerCount { get; set; } = 0;
		public int XP { get; set; }

		public string Rank { get; set; } = "Newbie";

		public DateTime SpecialDate { get; set; } = new DateTime();

		public string SpecialDateText => this.SpecialDate.ToString("dd.MM.yyyy");

		public string GetSpecialDateText => this.GetSpecDateText(SpecialDateDiff);

		public string SpecialDateDesc { get; set; } = "";

		private int SpecialDateDiff => (this.SpecialDate - DateTime.Today).Days;

		public IEnumerable<IZolik> Zoliky { get; set; }

		public string ClassName { get; set; }
		public IEnumerable<GetTopStudents_Result> LeaderboardClass { get; set; }
		public IEnumerable<GetTopStudents_Result> Leaderboard { get; set; }

		public string ZolikCountText
		{
			get
			{
				if (this.ZolikCount == 1) {
					// 1
					return "Vlastněný žolík";
				}

				if (this.ZolikCount >= 5 || this.ZolikCount == 0) {
					// 0, 5↑
					return "Vlastněných žolíků";
				}

				// 2, 3, 4
				return "Vlastnění žolíci";
			}
		}

		public string JokerCountText
		{
			get
			{
				if (this.JokerCount == 1) {
					// 1
					return "Jokér";
				}

				if (this.JokerCount >= 5 || this.JokerCount == 0) {
					// 0, 5↑
					return "Jokérů";
				}

				// 2, 3, 4
				return "Jokéři";
			}
		}

		private string GetSpecDateText(int diff)
		{
			if (diff >= 5) {
				return $"Zbývá {diff} dnů";
			}
			switch (diff) {
				case -1:
					return "Včera";
				case 0:
					return "Dnes";
				case 1:
					return "Zítra";
			}
			if (diff >= 2) {
				return $"Zbývají {diff} dny";
			}
			if (diff < -1) {
				return $"Před {diff * -1} dny";
			}
			return $"Zbývá {diff} (dny)";
		}

		public bool IsInClass => !string.IsNullOrWhiteSpace(this.ClassName);

		public bool IsPublic => HttpContext.Current.User.IsInRole(UserRoles.Public);
	}
}