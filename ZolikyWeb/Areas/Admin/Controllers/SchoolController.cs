using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary.Shared;
using Z.EntityFramework.Plus;
using ZolikyWeb.Areas.Admin.Models.School;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
	public class SchoolController : OwnController<SchoolManager>
	{
		public async Task<ActionResult> Dashboard()
		{
			var res = await Mgr.GetAllAsync();
			if (!res.Any()) {
				this.AddErrorToastMessage("Nebyly nalezeny žádné školy");
				return RedirectToApp();
			}
			return View(res);
		}

		public ActionResult Create()
		{
			return View();
		}

		public Task<ActionResult> Edit(int? id = null)
		{
			return EditOrDetail(id, true);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(SchoolModel.ID))]
		public async Task<ActionResult> Edit(SchoolModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			var ids = model.SubjectIds;

			return RedirectToAction("Edit", new {id = model.ID});
		}

		public Task<ActionResult> Detail(int? id = null)
		{
			return EditOrDetail(id, false);
		}

		private async Task<ActionResult> EditOrDetail(int? id, bool allowEdit)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}
			return await EditOrDetail((int) id, allowEdit);
		}

		private async Task<ActionResult> EditOrDetail(int id, bool allowEdit)
		{
			var res = await Mgr.GetByIdAsync(id);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Neplatná škola");
				return RedirectToAction("Dashboard");
			}
			var previousId = await Mgr.GetPreviousIdAsync(id);
			var nextId = await Mgr.GetNextIdAsync(id);

			var sMgr = this.GetManager<SubjectManager>();
			var allSubjects = await sMgr.GetAllAsync();

			return EditOrDetail(res.Content, allSubjects, allowEdit, previousId, nextId);
		}

		private ActionResult EditOrDetail(School school, List<Subject> allSubjects, bool allowEdit, int previousId,
										  int nextId)
		{
			var model = new SchoolModel(school, allSubjects, allowEdit, previousId, nextId);

			return View("Edit", model);
		}
	}
}