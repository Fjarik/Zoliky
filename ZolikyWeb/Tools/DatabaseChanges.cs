using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers.New;
using DataAccess.Models;
using SharedLibrary.Shared;

namespace ZolikyWeb.Tools
{
	public class DatabaseChanges
	{
		/*[HttpGet]
		public async Task<ActionResult> Delete()
		{
			var id = 1128;
			var res = await Mgr.DeleteAsync(id);
			return RedirectToLogin();
		}*/

		/*[HttpGet]
		public async Task<ActionResult> Test()
		{
			var mgr = this.GetManager<UserSettingManager>();
			using (var ent = new ZoliksEntities()) {
				var consents = ent.Consents.Where(x => x.TermID == 3 || x.TermID == 5);
				var groups = consents.GroupBy(x => x.UserID);

				foreach (var group in groups) {
					var id = group.Key;
					foreach (var consent in group) {
						string key;
						switch (consent.TermID) {
							case 3:
								key = SettingKeys.Newsletter;
								break;
							case 5:
								key = SettingKeys.FutureNews;
								break;
							default:
								return RedirectToLogin();
						}
						var createRes = await mgr.CreateAsync(id, key, consent.Accepted.ToString(), true);
						if (!createRes.IsSuccess) {
							return RedirectToLogin();
						}
					}
				}
			}
			return RedirectToLogin();
		}*/

		/*
		[HttpGet]
		public async Task<ActionResult> Test()
		{
			var mgr = this.GetManager<UserSettingManager>();
			var keys = new List<string> { SettingKeys.VisibleRank, SettingKeys.LeaderboardZolik, SettingKeys.VisibleZolik };
			using (var ent = new ZoliksEntities()) {
				foreach (var userId in ent.Users.Select(x => x.ID)) {
					foreach (var key in keys) {
						await mgr.CreateAsync(userId, key, true);
					}
				}
			}
			return RedirectToLogin();
		}
		*/

		/*
		[HttpGet]
		public async Task<ActionResult> Test()
		{
			var mgr = this.GetManager<UserSettingManager>();
			var key = SettingKeys.MobileToken;
			using (var ent = new ZoliksEntities()) {
				foreach (var user in ent.Users
										.Where(x => x.MobileToken.Trim() != null)
										.Select(x => new {x.ID, x.MobileToken})) {
					await mgr.CreateAsync(user.ID, key, user.MobileToken, true);
				}
			}
			return RedirectToLogin();
		}
		*/

	}
}