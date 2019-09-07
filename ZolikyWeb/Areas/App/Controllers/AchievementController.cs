using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.App.Models.Achievement;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Controllers
{
	[Authorize]
	public class AchievementController : OwnController<AchievementManager>
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public async Task<ActionResult> Dashboard()
		{
			var id = this.User.Identity.GetId();

			var sMgr = this.GetManager<UserSettingManager>();
			var set = await sMgr.GetStringValueAsync(id, SettingKeys.LastAchievementsCheck);
			if (!string.IsNullOrEmpty(set) && DateTime.TryParse(set, out DateTime lastCheck)) {
				ViewBag.LastCheck = lastCheck;
			}

			var achs = await Mgr.GetAllAsync();
			var unlocked = await Mgr.GetUnlockedIdsAsync(id);
			var ids = unlocked.Select(x => x.AchievementId);

			var model = achs.Where(x => ids.All(y => x.ID != y)).Select(x => new AchievementModel(x));

			var un = unlocked.Select(x => new AchievementModel(achs.FirstOrDefault(y => y.ID == x.AchievementId)) {
										 IsUnlocked = true,
										 When = x.When
									 }
									);

			var res = un.Concat(model);

			return View(res);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CheckAchievements()
		{
			var id = this.User.Identity.GetId();

			var sMgr = this.GetManager<UserSettingManager>();
			var set = await sMgr.GetStringValueAsync(id, SettingKeys.LastAchievementsCheck);
			var now = DateTime.Now;
			if (!string.IsNullOrEmpty(set) &&
				DateTime.TryParse(set, out DateTime lastCheck) &&
				(lastCheck - now).Days < 1) {
				this.AddErrorToastMessage("Tuto akci lze spustit pouze 1x za den!");
				return RedirectToAction("Dashboard");
			}
			// TODO: Check
			var aMgr = this.GetManager<AchievementManager>();

			await sMgr.CreateAsync(id,
								   SettingKeys.LastAchievementsCheck,
								   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
								   true);
			return RedirectToAction("Dashboard");
		}
	}
}