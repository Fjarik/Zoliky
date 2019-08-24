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
using ZolikyWeb.Areas.Admin.Models.Subject;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
	public class SubjectController : OwnController<SchoolManager>
	{
		public async Task<ActionResult> Dashboard(bool? onlyAssigned)
		{
			var schoolId = this.User.GetSchoolId();

			var subjects = await Mgr.GetSubjectsAsync(schoolId);

			var assigned = onlyAssigned != null;
			if (assigned) {
				subjects = subjects.Where(x => x.GetTeacherCount(schoolId) > 0).ToList();
			}
			
			var model = new SubjectDashboardModel(subjects, assigned);
			return View(model);
		}

		public Task<ActionResult> Edit(int? id = null, int? teacherId = null)
		{
			return EditAsync(id, teacherId);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(SubjectEditModel model)
		{
			if (model == null || !model.IsValid) {
				this.AddErrorToastMessage("Neplatné úpravy");
				return RedirectToAction("Edit");
			}

			var sMgr = this.GetManager<TeacherSubjectManager>();
			var teacherId = model.TeacherID;
			var subjectId = model.SubjectID;

			await sMgr.DeleteAsync(teacherId, subjectId);
			foreach (var classId in model.ClassIDs) {
				var res = await sMgr.CreateAsync(teacherId, subjectId, classId);
				if (!res.IsSuccess) {
					this.AddErrorToastMessage($"Vyskytla se chyba: {res.GetStatusMessage()}");
					return RedirectToAction("Edit", new {id = subjectId, teacherId});
				}
			}
			this.AddSuccessToastMessage("Úspěšně uloženo");
			return RedirectToAction("Edit", new {id = subjectId, teacherId});
		}

		private async Task<ActionResult> EditAsync(int? id, int? teacherId)
		{
			var schoolId = this.User.GetSchoolId();

			var cMgr = this.GetManager<ClassManager>();

			var subjects = await Mgr.GetSubjectsAsync(schoolId);
			var teachers = (await Mgr.GetSchoolTeachersAsync(schoolId)).ToList<IStudent>();
			var classes = (await cMgr.GetAllAsync(schoolId, true)).ToList<IClass>();

			var model = new SubjectEditModel(subjects, teachers, classes) {
				SubjectID = id ?? -1,
				TeacherID = teacherId ?? -1
			};

			return View("Edit", model);
		}

		public async Task<JsonResult> GetTeacherSubjectClasses(int subjectId, int teacherId)
		{
			var sMgr = this.GetManager<TeacherSubjectManager>();
			var res = await sMgr.GetTeacherClassIdsAsync(teacherId, subjectId);
			return Json(res, JsonRequestBehavior.AllowGet);
		}
	}
}