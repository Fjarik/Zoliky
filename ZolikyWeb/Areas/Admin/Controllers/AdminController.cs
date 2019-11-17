using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using SharedLibrary.Enums;
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
			var classes = await Mgr.GetClassLeaderboardAsync(schoolId);
			var subjects = await Mgr.GetSubjectsAsync(schoolId);

			var pMgr = this.GetManager<ProjectSettingManager>();
			var specDate = await pMgr.GetStringValueAsync(null, ProjectSettingKeys.SpecialDate) ??
						   DateTime.Now.ToString();
			var specTitle = await pMgr.GetStringValueAsync(null, ProjectSettingKeys.SpecialText) ?? "";

			var model = new DashboardModel(subjects) {
				SchoolStudentsCount = students,
				SchoolTeachersCount = teachers,
				SchoolZoliksCount = zoliks,
				SpecialDate = DateTime.Parse(specDate),
				SpecialDateDesc = specTitle,
				ClassesLeaderboard = classes.Take(10)
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

		[HttpGet]
		public async Task<JsonResult> GetClassJson()
		{
			var schoolId = this.User.GetSchoolId();
			var classes = await Mgr.GetClassLeaderboardAsync(schoolId);

			var res = classes.OrderBy(x => x.Name).Select(x => new {
				label = x.Name,
				data = new[] {
					x.ZolikCount > 0 ? x.ZolikCount : 0.1
				},
				backgroundColor = x.Colour
			});

			return Json(res, JsonRequestBehavior.AllowGet);
		}

		// Poměr žolíků:Jokérů:Černých petrů
		[HttpGet]
		public async Task<JsonResult> GetZoliksJson()
		{
			var schoolId = this.User.GetSchoolId();

			var zoliks = await Mgr.GetZoliksGraphDataAsync(schoolId);

			return Json(zoliks, JsonRequestBehavior.AllowGet);
		}
	}
}