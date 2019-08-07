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
			var schoolId = this.User.Identity.GetSchoolId();

			var subjects = await Mgr.GetSubjectsAsync(schoolId);

			var assigned = onlyAssigned != null;

			var model = new SubjectDashboardModel(subjects, assigned);
			return View(model);
		}

		public Task<ActionResult> Edit(int? id = null)
		{
			if (id == null || id < 1) {
				return EditAsync(-1);
			}
			return EditAsync((int) id);
		}

		private async Task<ActionResult> EditAsync(int id)
		{
			var schoolId = this.User.Identity.GetSchoolId();

			var cMgr = this.GetManager<ClassManager>();

			var subjects = await Mgr.GetSubjectsAsync(schoolId);
			var teachers = (await Mgr.GetSchoolTeachersAsync(schoolId)).ToList<IStudent>();
			var classes = (await cMgr.GetAllAsync(schoolId, true)).ToList<IClass>();

			var model = new SubjectEditModel(subjects, teachers, classes) {
				SubjectID = id
			};

			return View("Edit", model);
		}

		public async Task<JsonResult> GetTeacherSubjectClasses(int subjectId, int teacherId)
		{
			var sMgr = this.GetManager<TeacherSubjectManager>();
			var res = await sMgr.GetTeacherClassesAsync(teacherId, subjectId);
			return Json(res, JsonRequestBehavior.AllowGet);
		}

	}
}