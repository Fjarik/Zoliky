using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Admin.Models.Admin;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
	public class AdminController : OwnController<SchoolManager>
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public async Task<ActionResult> Dashboard()
		{
			var schoolId = this.User.GetSchoolId();

			var students = await Mgr.GetStudentCountAsync(schoolId);
			var teachers = await Mgr.GetTeacherCountAsync(schoolId);
			var zoliks = await Mgr.GetZolikCountAsync(schoolId);

			var model = new DashboardModel {
				SchoolStudentsCount = students,
				SchoolTeachersCount = teachers,
				SchoolZoliksCount = zoliks,
				SpecialDate = new DateTime(2019, 09, 02),
				SpecialDateDesc = "Nový školní rok"
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ChangeSchool(ChangeSchoolModel model)
		{
			this.Session[Ext.Session.SchoolID] = model.SchoolId;
			return RedirectToAction("Dashboard");
		}

	}
}