using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DataAccess.Models;
using Microsoft.Owin.Security;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.App.Models.Account;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Models
{
	public class SettingsModel
	{
		[Display(Name = "Testerské funkce")]
		public bool IsTester { get; set; }

		public int TabId { get; set; } = 1;

		public bool HasTesterRights { get; set; }

		public GeneralModel GeneralModel { get; set; }

		public PrivacyModel PrivacyModel { get; set; }

		public NotificationModel NotificationModel { get; set; }

		public ExternalModel ExternalModel { get; set; }

		public SettingsModel(User u, bool isTester, ICollection<UserSetting> settings, int tabId)
		{
			this.IsTester = isTester;
			this.HasTesterRights = u.IsInRole(UserRoles.Tester);
			this.TabId = tabId;
			this.GeneralModel = new GeneralModel(u);
			this.PrivacyModel = new PrivacyModel
			{
				ZolikVisible = settings.GetValue<bool>(SettingKeys.VisibleZolik),
				ZolikLeaderboard = settings.GetValue<bool>(SettingKeys.LeaderboardZolik),
				RankVisible = settings.GetValue<bool>(SettingKeys.VisibleRank),
				XpLeaderboard = settings.GetValue<bool>(SettingKeys.LeaderboardXp),
			};
			this.NotificationModel = new NotificationModel {
				Newsletter = settings.GetValue<bool>(SettingKeys.Newsletter),
				FutureNews = settings.GetValue<bool>(SettingKeys.FutureNews)
			};
			this.ExternalModel = new ExternalModel(u.LoginTokens.ToList());
		}

		
	}
}