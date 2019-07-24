using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using ZolikyWeb.Models.Base;

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

			return View(res);
		}
	}
}