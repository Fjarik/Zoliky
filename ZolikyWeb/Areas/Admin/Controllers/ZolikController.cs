using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary;
using SharedLibrary.Enums;
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
		public async Task<ActionResult> Dashboard(bool? onlyEnabled, int? classId = null)
		{
			var sMgr = this.GetManager<SchoolManager>();

			var schoolId = this.User.GetSchoolId();
			var isTester = this.User.Identity.IsTester();

			bool active = onlyEnabled != null;
			var classes = await sMgr.GetClassesAsync(schoolId);

			var res = await Mgr.GetSchoolZoliksAsync(schoolId, active, isTester);
			var model = new ZolikDashboardModel(res, classes, active, classId);
			return View(model);
		}

		//public async Task<ActionResult> RemoveOld()
		//{
		//	var date = Ext.SchoolYearStart;
		//	var all = await Mgr.GetAllAsync();
		//	var a = all.Where(x => x.Enabled &&
		//						   x.Created < date);
		//	var teacherId = this.User.Identity.GetId();

		//	foreach (var zolik in a) {
		//		var res = await Mgr.RemoveAsync(zolik.ID, "Již neplatný žolík", teacherId);
		//		if (!res.IsSuccess) {
		//			this.AddErrorToastMessage("Vyskytla se chyba: " + res.GetStatusMessage());
		//			return RedirectToAction("Dashboard");
		//		}
		//	}

		//	this.AddSuccessToastMessage("Vše OK");
		//	return RedirectToAction("Dashboard");
		//}

#region Create

		public async Task<ActionResult> Create(string title = null, ZolikType? type = null, int? subjectId = null)
		{
			var logged = await this.GetLoggedUserAsync();
			var schoolId = this.User.GetSchoolId();
			//var schools = await this.GetSchoolAsync();

			// Pouze škola registrovaného správce školy - Nemůže přidat třídu do cizí školy
			var sMgr = this.GetManager<SchoolManager>();
			var subjects = await sMgr.GetSubjectsAsync(schoolId);
			var students = await sMgr.GetStudentsAsync(schoolId, logged.ID);
			var isTester = this.User.Identity.IsTester();

			var model = ZolikModel.CreateModel(logged, subjects, students.ToList<IUser>(), isTester);
			if (!string.IsNullOrWhiteSpace(title)) {
				model.Title = title;
			}
			if (type != null) {
				model.Type = (ZolikType) type;
			}
			if (subjectId != null) {
				model.SubjectID = (int) subjectId;
			}
			return View("Edit", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ZolikModel.ID))]
		public async Task<ActionResult> Create(ZolikModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			if (!model.StudentIds.Any()) {
				this.AddErrorToastMessage("Musíte vybrat minimálně jednoho studenta jako vlastníka");
				return RedirectToAction("Create",
										new {title = model?.Title, type = model?.Type, subjectId = model?.SubjectID});
			}

			if (!model.IsValid) {
				this.AddErrorToastMessage("Neplatné hodnoty");
				return RedirectToAction("Create");
			}
			var logged = await this.GetLoggedUserAsync();
			var toId = model.OwnerID;
			if (logged.ID == toId) {
				this.AddErrorToastMessage("Nemůžete poslat žolíka sám sobě");
				return RedirectToAction("Create");
			}

			MActionResult<Transaction> res = new MActionResult<Transaction>(StatusCode.InternalError);
			foreach (var studentId in model.StudentIds) {
				res = await Mgr.CreateAndTransferAsync(logged.ID,
													   studentId,
													   model.Type,
													   model.SubjectID,
													   model.Title,
													   logged,
													   model.AllowSplit);
				if (!res.IsSuccess) {
					this.AddErrorToastMessage($"Nezdařilo se vytvořit záznam. Chyba: {res.GetStatusMessage()}");
					return RedirectToAction("Dashboard");
				}
			}

			if (!res.IsSuccess) {
				this.AddErrorToastMessage($"Bylo vybráno málo studentů");
				return RedirectToAction("Dashboard");
			}

			var original = res.Content;

			this.AddSuccessToastMessage("Žolíci byli úspěšně vytvořeni");
			return RedirectToAction("Detail", new {id = original.ZolikID});
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
			original.Title = model.Title;
			original.Type = model.Type;
			original.SubjectID = model.SubjectID;
			original.Enabled = model.Enabled;
			original.AllowSplit = model.AllowSplit;
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
			var schoolId = this.User.GetSchoolId();

			var previousId = await Mgr.GetPreviousIdAsync(id);
			var nextId = await Mgr.GetNextIdAsync(id);
			var isTester = this.User.Identity.IsTester();

			var sMgr = this.GetManager<SchoolManager>();

			var subjects = await sMgr.GetSubjectsAsync(schoolId);


			var sRes = await sMgr.GetByIdAsync(schoolId);
			var allowRemove = false;
			if (sRes.IsSuccess) {
				allowRemove = sRes.Content.AllowTeacherRemove;
			}

			var model = new ZolikModel(res.Content, subjects, allowRemove, allowEdit, previousId, nextId, isTester) {
				ActionName = actionName
			};

			return View("Edit", model);
		}

#endregion

#endregion

#region Remove

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ZolikRemoveModel.ID), nameof(ZolikRemoveModel.OwnerID))]
		public Task<ActionResult> Remove(ZolikRemoveModel model)
		{
			return RemoveAsync(model?.Reason, model?.ID, model?.OwnerID);
		}

		private async Task<ActionResult> RemoveAsync(string reason, int? id = null, int? ownerId = null)
		{
			if (id == null || id < 1 || ownerId == null || ownerId < 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}
			if (string.IsNullOrWhiteSpace(reason)) {
				this.AddErrorToastMessage("Neplatný důvod");
				return RedirectToAction("Dashboard");
			}
			return await RemoveAsync(reason, (int) id, (int) ownerId);
		}

		private async Task<ActionResult> RemoveAsync(string reason, int id, int ownerId)
		{
			var logged = await this.GetLoggedUserAsync();

			var res = await Mgr.RemoveAsync(id, ownerId, reason, logged);
			if (res.IsSuccess) {
				this.AddSuccessToastMessage("Žolík byl úspěšně odstraněn");
			} else {
				this.AddErrorToastMessage($"Nezdařilo se odstranit žolíka, chyba: {res.GetStatusMessage()}");
			}
			return RedirectToAction("Dashboard");
		}

#endregion
	}
}