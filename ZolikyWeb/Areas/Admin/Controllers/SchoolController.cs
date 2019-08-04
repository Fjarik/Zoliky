using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary.Enums;
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

#region Create

		public Task<ActionResult> Create()
		{
			return CreateAsync();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(SchoolModel.ID))]
		public async Task<ActionResult> Create(SchoolModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			if (!model.IsValid) {
				this.AddErrorToastMessage("Neplatné hodnoty");
				return RedirectToAction("Edit");
			}

			var res = await Mgr.CreateAsync((SchoolTypes) model.TypeID,
											model.Name,
											model.Street,
											model.City,
											model.AllowTransfer,
											model.AllowTeacherRemove,
											model.AllowZolikSplik);

			if (!res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se vytvořit záznam. Chyba: {res.GetStatusMessage()}");
				return RedirectToAction("Dashboard");
			}
			var original = res.Content;
			foreach (var add in model.SubjectIds) {
				var addRes = await Mgr.CreateSchoolSubjectAsync(original.ID, add);
			}
			this.AddSuccessToastMessage("Škola byla úspěšně vytvořena");
			return RedirectToAction("Detail", new {id = original.ID});
		}

		private async Task<ActionResult> CreateAsync()
		{
			var sMgr = this.GetManager<SubjectManager>();
			var allSubjects = await sMgr.GetAllAsync();
			return Create(allSubjects);
		}

		private ActionResult Create(List<Subject> allSubjects)
		{
			var model = new SchoolModel(allSubjects);
			return View("Edit", model);
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
		[ValidateSecureHiddenInputs(nameof(SchoolModel.ID))]
		public async Task<ActionResult> Edit(SchoolModel model)
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

			// Add subject "Jiné"
			var sIds = model.SubjectIds;
			if (!sIds.Contains(4)) {
				sIds.Add(4);
			}

			var original = res.Content;

			// Edit
			original.Type = (SchoolTypes) model.TypeID;
			original.Name = model.Name;
			original.Street = model.Street;
			original.City = model.City;
			original.AllowTransfer = model.AllowTransfer;
			original.AllowTeacherRemove = model.AllowTeacherRemove;
			original.AllowZolikSplik = model.AllowZolikSplik;

			var sMgr = this.GetManager<SubjectManager>();

			var idsToRemove = original.SchoolSubjects
									  .Where(x => !sIds.Contains(x.SubjectID))
									  .Select(x => x.SubjectID)
									  .ToList();
			var idsToAdd = sIds.Where(x => original.SchoolSubjects
												   .All(y => y.SubjectID != x))
							   .ToList();

			var removeRes = await Mgr.RemoveSchoolSubjectsAsync(original.ID, idsToRemove);

			foreach (var add in idsToAdd) {
				var addRes = await Mgr.CreateSchoolSubjectAsync(original.ID, add);
			}
			// Edit end

			await Mgr.SaveAsync(original);
			this.AddSuccessToastMessage("Úspěšně uloženo");
			return RedirectToAction("Edit", new {id = model.ID});
		}

#endregion

#region Detail

		public Task<ActionResult> Detail(int? id = null)
		{
			return EditOrDetail(id, "Detail", false);
		}

#endregion

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
			var previousId = await Mgr.GetPreviousIdAsync(id);
			var nextId = await Mgr.GetNextIdAsync(id);

			var sMgr = this.GetManager<SubjectManager>();
			var allSubjects = await sMgr.GetAllAsync();

			return EditOrDetail(res.Content, allSubjects, actionName, allowEdit, previousId, nextId);
		}

		private ActionResult EditOrDetail(School school,
										  List<Subject> allSubjects,
										  string actionName,
										  bool allowEdit,
										  int previousId,
										  int nextId)
		{
			var model = new SchoolModel(school, allSubjects, allowEdit, previousId, nextId) {
				ActionName = actionName
			};

			return View("Edit", model);
		}

#endregion

#region Remove

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(School.ID))]
		public Task<ActionResult> Remove(School model)
		{
			return RemoveAsync(model?.ID);
		}

		private async Task<ActionResult> RemoveAsync(int? id = null)
		{
			if (id == null || id <= 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}
			return await RemoveAsync((int) id);
		}

		private async Task<ActionResult> RemoveAsync(int id)
		{
			var res = await Mgr.DeleteAsync(id);

			if (res) {
				this.AddSuccessToastMessage("Škola úspěšně odstraněna");
			} else {
				this.AddErrorToastMessage("Nezdařilo se odstranit školu");
			}

			return RedirectToAction("Dashboard");
		}

#endregion
	}
}