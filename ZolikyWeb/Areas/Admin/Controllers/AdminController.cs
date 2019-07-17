using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Admin.Models;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper + "," + UserRoles.Teacher)]
	public class AdminController : OwnController
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public ActionResult Dashboard()
		{
			var model = new SendNotificationModel() {
				ToId = 2
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendNotification(SendNotificationModel model)
		{
			var fMgr = this.GetManager<FirebaseManager>();

			var id = User.Identity.GetId();
			var res = await fMgr.NewZolikAsync(model.ZolikId, id, model.ToId);
			if (res) {
				this.AddSuccessToastMessage("Úspěch");
			}

			return View("Dashboard", model);
		}
	}
}