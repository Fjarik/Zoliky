using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Models;
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
	}
}