using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedLibrary.Shared;

namespace ZolikyWeb.Areas.App.Models.Account
{
	public class PrivacyModel
	{
		public bool ZolikVisible { get; set; }
		public bool ZolikLeaderboard{ get; set; }
		public bool RankVisible { get; set; }
		public bool XpLeaderboard { get; set; }

		public IDictionary<string, bool> Dictionary => new ConcurrentDictionary<string, bool>() {
			[SettingKeys.VisibleZolik] = this.ZolikVisible,
			[SettingKeys.LeaderboardZolik] = this.ZolikLeaderboard,
			[SettingKeys.VisibleRank] = this.RankVisible,
			[SettingKeys.LeaderboardXp] = this.XpLeaderboard,
		};
	}
}