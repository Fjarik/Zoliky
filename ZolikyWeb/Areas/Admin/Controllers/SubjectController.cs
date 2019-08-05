using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Admin.Models.Subject;
using ZolikyWeb.Models.Base;

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
    }
}