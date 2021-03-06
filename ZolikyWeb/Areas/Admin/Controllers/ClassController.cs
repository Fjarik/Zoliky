﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Admin.Models.Class;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
	public class ClassController : OwnController<ClassManager>
	{
		public async Task<ActionResult> Dashboard(bool? onlyActive)
		{
			var schoolId = this.User.GetSchoolId();

			bool active = onlyActive != null;

			var res = await Mgr.GetAllAsync(schoolId, active);
			if (!res.Any()) {
				this.AddErrorToastMessage("Nebyly nalezeny žádné třídy");
				return RedirectToAction("Create");
			}
			var model = new ClassDashboardModel(res, active);
			return View(model);
		}

#region Create

		public async Task<ActionResult> Create()
		{
			var schoolId = this.User.GetSchoolId();
			//var schools = await this.GetSchoolAsync();

			var model = ClassModel.CreateModel();
			return View("Edit", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ClassModel.ID))]
		public async Task<ActionResult> Create(ClassModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			var schoolId = this.User.GetSchoolId();
			model.SchoolID = schoolId;

			if (!model.IsValid) {
				this.AddErrorToastMessage("Neplatné hodnoty");
				return RedirectToAction("Create");
			}

			var res = await Mgr.CreateAsync(model.Name,
											model.SchoolID,
											model.Since,
											model.Graduation,
											model.Colour);

			if (!res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se vytvořit záznam. Chyba: {res.GetStatusMessage()}");
				return RedirectToAction("Dashboard");
			}
			var original = res.Content;

			this.AddSuccessToastMessage("Třída byla úspěšně vytvořena");
			return RedirectToAction("Detail", new {id = original.ID});
		}

#endregion

#region Edit & Detail

#region Edit

		public Task<ActionResult> Edit(int? id = null)
		{
			return EditOrDetail(id, "Edit", true);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ClassModel.ID))]
		public async Task<ActionResult> Edit(ClassModel model)
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
			var colour = model.Colour;
			if (!colour.StartsWith("#")) {
				colour = $"#{colour}";
			}

			var original = res.Content;

			// Edit
			original.Name = model.Name;
			original.Since = model.Since;
			original.Graduation = model.Graduation;
			original.Colour = colour;
			// Edit end

			await Mgr.SaveAsync(original);
			this.AddSuccessToastMessage("Úspěšně uloženo");
			return RedirectToAction("Detail", new {id = model.ID});
		}

#endregion

#region Detail

		public Task<ActionResult> Detail(int? id = null)
		{
			return EditOrDetail(id, "Edit", false);
		}

#endregion

#region EditOrDetails

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
				this.AddErrorToastMessage("Neplatná škola");
				return RedirectToAction("Dashboard");
			}
			var schoolId = this.User.GetSchoolId();
			var cl = res.Content;
			if (cl.SchoolID != schoolId) {
				this.AddErrorToastMessage("Nemáte oprávnění upravovat tuto třídu");
				return RedirectToAction("Dashboard");
			}

			var previousId = await Mgr.GetPreviousClassIdAsync(id, schoolId);
			var nextId = await Mgr.GetNextClassIdAsync(id, schoolId);
			var names = await Mgr.GetStudentNamesAsync(id);

			var model = new ClassModel(cl, names, allowEdit, previousId, nextId) {
				ActionName = actionName
			};

			return View("Edit", model);
		}

#endregion

#endregion

#region Remove

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ClassModel.ID))]
		public Task<ActionResult> Remove(ClassModel model)
		{
			return RemoveAsync(model?.ID);
		}

		private async Task<ActionResult> RemoveAsync(int? id = null)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}
			return await RemoveAsync((int) id);
		}

		private async Task<ActionResult> RemoveAsync(int id)
		{
			var res = await Mgr.DeleteAsync(id);
			if (res) {
				this.AddSuccessToastMessage("Třída úspěšně odstraněna");
			} else {
				this.AddErrorToastMessage("Nezdařilo se odstranit třídu");
			}

			return RedirectToAction("Dashboard");
		}

#endregion
	}
}