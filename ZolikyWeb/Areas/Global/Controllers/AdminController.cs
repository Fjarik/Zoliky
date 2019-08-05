using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Global.Models.Admin;
using ZolikyWeb.Areas.Global.Models.Admin.Dashboard;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper)]
	public class AdminController : OwnController
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public ActionResult Dashboard()
		{
			var model = new DashboardModel(2);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendMobileNotification(SendMobileNotModel model)
		{
			var fMgr = this.GetManager<FirebaseManager>();

			var id = User.Identity.GetId();
			var res = await fMgr.NewZolikAsync(model.ZolikId, id, model.ToId);
			if (res) {
				this.AddSuccessToastMessage("Úspěch");
			}

			return RedirectToAction("Dashboard");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendNotifications(SendNotificationsModel model)
		{
			if (model != null && model.IsValid) {
				var nMgr = this.GetManager<NotificationManager>();

				var res = await nMgr.SendNotificationToStudentsAsync(model.Title,
																	 model.Subtitle);
				if (!res) {
					this.AddErrorToastMessage("Něco se nepovedlo");
				}
			}

			return RedirectToAction("Dashboard");
		}
	}
}