using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using SharedLibrary.Interfaces;
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
			var model = new ZolikDashboardModel(res, active);
			return View(model);
		}

#region Create

#endregion

#region Edit & Detail

#region Edit

		public Task<ActionResult> Edit(int? id = null)
		{
			return EditOrDetail(id, "Edit", true);
		}

#endregion

#region Detail

		public Task<ActionResult> Detail(int? id = null)
		{
			return EditOrDetail(id, "Edit", false);
		}

#endregion

#region Edit or Detail

		private async Task<ActionResult> EditOrDetail(int? id, string actionName, bool allowEdit)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}
			return await EditOrDetail((int) id, actionName, allowEdit);
		}

		private async Task<ActionResult> EditOrDetail(int id, string actionName, bool allowEdit)
		{
			var res = await Mgr.GetByIdAsync(id);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Neplatný žolík");
				return RedirectToAction("Dashboard");
			}
			var schoolId = this.User.Identity.GetSchoolId();

			var previousId = await Mgr.GetPreviousIdAsync(id);
			var nextId = await Mgr.GetNextIdAsync(id);


			var sMgr = this.GetManager<SchoolManager>();

			var students = new List<IUser>();
			var subjects = await sMgr.GetSubjectsAsync(schoolId);

			var model = new ZolikModel(res.Content, students, subjects, allowEdit, previousId, nextId) {
				ActionName = actionName
			};

			return View("Edit", model);
		}

#endregion

#endregion

#region Remove

#endregion
	}
}