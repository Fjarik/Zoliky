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

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ZolikModel.ID))]
		public async Task<ActionResult> Edit(ZolikModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			if (!model.IsValid) {
				this.AddErrorToastMessage("Neplatné hodnoty");
				return RedirectToAction("Edit");
			}

			var res = await Mgr.GetByIdAsync(model.ID);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se načíst originální záznam. Chyba: {res.GetStatusMessage()}");
				return RedirectToAction("Dashboard");
			}
			var original = res.Content;

			// Edit
			// Edit end

			await Mgr.SaveAsync(original);
			this.AddSuccessToastMessage("Úspěšně uloženo");
			return RedirectToAction("Edit", new {id = model.ID});
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

			var subjects = await sMgr.GetSubjectsAsync(schoolId);

			var model = new ZolikModel(res.Content, subjects, allowEdit, previousId, nextId) {
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