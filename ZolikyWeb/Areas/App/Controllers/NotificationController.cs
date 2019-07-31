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
	public class NotificationController : OwnController<NotificationManager>
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public async Task<ActionResult> Dashboard()
		{
			var id = this.User.Identity.GetId();

			var res = await Mgr.GetUserNotificationsAsync(id);
			if (!res.Any()) {
				this.AddErrorToastMessage("Nemáte žádná oznámení");
				return RedirectToApp();
			}
			return View(res);
		}

		[HttpPost]
		public async Task<JsonResult> NotificationSeen(int id)
		{
			if (id < 1) {
				return Json(false);
			}

			var userId = this.User.Identity.GetId();

			var res = await Mgr.SeenNotificationAsync(id, userId);
			return Json(res);
		}

		public async Task<PartialViewResult> GetList()
		{
			var id = this.User.Identity.GetId();
			var res = await Mgr.GetUserNotificationsAsync(id);
			return PartialView(res);
		}

		public async Task<JsonResult> UnseenNotification()
		{
			var id = this.User.Identity.GetId();

			var res = await Mgr.AnyUnseenNotificationAsync(id);

			return Json(res, JsonRequestBehavior.AllowGet);
		}
	}
}