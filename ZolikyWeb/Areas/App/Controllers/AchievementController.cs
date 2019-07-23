using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
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

			var res = await Mgr.GetAllAsync();
			return View(res);
		}
	}
}