using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Admin.Models.Zoliky;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
    public class ZolikController : OwnController<ZolikManager>
	{
		public async Task<ActionResult> Dashboard(bool? onlyEnabled)
		{
			var schoolId = this.User.Identity.GetSchoolId();

			bool active = onlyEnabled != null;

			var res = await Mgr.GetSchoolZoliksAsync(schoolId, active);
			//if (!res.Any()) {
			//	this.AddErrorToastMessage("Nebyly nalezeny žádní žolíci");
			//	return RedirectToApp();
			//}
			var model = new ZolikDashboardModel(res, active);
			return View(model);
		}
	}
}