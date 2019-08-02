using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary.Shared;
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
			var res = await Mgr.GetByIdAsync((int)id);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Neplatná škola");
				return RedirectToAction("Dashboard");
			}
			return EditOrDetail(res.Content, allowEdit);
		}

		private ActionResult EditOrDetail(School school, bool allowEdit)
		{
			var model = new  SchoolModel(school, allowEdit);

			return View("Edit", model);
		}

	}
}