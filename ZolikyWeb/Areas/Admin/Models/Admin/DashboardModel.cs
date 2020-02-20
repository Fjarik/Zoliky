using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess;
using DataAccess.Models;

namespace ZolikyWeb.Areas.Admin.Models.Admin
{
	public class DashboardModel
	{
		public int SchoolStudentsCount { get; set; }
		public int SchoolTeachersCount { get; set; }
		public int SchoolZoliksCount { get; set; }

		public QuickZolik QuickZolik { get; set; }

		public IEnumerable<ClassLeaderboard> ClassesLeaderboard { get; set; } = new List<ClassLeaderboard>();

#region Special date

		public DateTime SpecialDate { get; set; } = new DateTime();
		public string SpecialDateText => this.SpecialDate.ToString("dd.MM.yyyy");
		public string GetSpecialDateText => this.GetSpecDateText(SpecialDateDiff);
		public string SpecialDateDesc { get; set; } = "";
		private int SpecialDateDiff => (this.SpecialDate - DateTime.Today).Days;

#endregion

		public DashboardModel() : this(new List<ZolikType>(), new List<DataAccess.Models.Subject>()) { }

		public DashboardModel(List<ZolikType> types,
							  List<DataAccess.Models.Subject> subjects)
		{
			this.QuickZolik = new QuickZolik(types, subjects);
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
	}
}