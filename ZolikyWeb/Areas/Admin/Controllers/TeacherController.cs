using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using SharedLibrary.Shared;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
    public class TeacherController : OwnController<SchoolManager>
	{
		public async Task<ActionResult> Dashboard()
		{
			var schoolId = this.User.GetSchoolId();

			var res = await this.Mgr.GetSchoolTeachersAsync(schoolId, false);
			if (!res.Any()) {
				this.AddErrorToastMessage("Nebyly nalezeny žádní vyučující");
				return RedirectToAction("Create");
			}
			return View(res);
		}

		public ActionResult Create()
		{
			return View();
		}
    }
}