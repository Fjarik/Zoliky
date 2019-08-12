using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using SharedLibrary.Enums;
using SharedLibrary.Shared;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
	public class StudentController : OwnController<UserManager>
	{
#region Student list (Dashboard)

		public ActionResult Dashboard()
		{
			return View();
		}

		public async Task<JsonResult> UsersJson()
		{
			var schoolId = this.User.Identity.GetSchoolId();
			var loggedId = this.User.Identity.GetId();

			var sMgr = this.GetManager<SchoolManager>();

			var users = await sMgr.GetStudentsAsync(schoolId);


			var res = users.OrderBy(x => x.ClassName)
						   .ThenBy(x => x.Lastname)
						   .ThenBy(x => x.Name)
						   .ThenBy(x => x.Name)
						   .Select(x => new {
							   id = x.ID,
							   fullname = x.FullName,
							   email = x.Email,
							   @class = new {
								   name = x.Class.Name,
								   since = x.Class.Since.Year,
								   grad = x.Class.Graduation.Year
							   },
							   zolikCount = x.OriginalZoliks.Count(y => !y.Type.IsTesterType() && y.Enabled),
							   memberSince = new {
								   date = x.MemberSince.ToString("dd.MM.yyyy"),
								   timeS = x.MemberSince.GetJsTimestamp(),
								   regIp = x.RegistrationIp
							   },
							   lastLogin = x.LastLogin == null
											   ? new {
												   date = "",
												   timeS = 0.0,
												   project = ""
											   }
											   : new {
												   date = x.LastLogin.Date.ToString("dd.MM.yyyy"),
												   timeS = x.LastLogin.Date.GetJsTimestamp(),
												   project = ((Projects) x.LastLogin.ProjectID).GetDescription()
											   },
							   x.IsBanned,
							   x.IsEnabled,
							   actions = new {
								   edit = x.ID == loggedId
											  ? ""
											  : Url.Action("Edit", "User", new {Area = "Admin", id = x.ID}),
								   display = Url.Action("Detail", "User", new {Area = "Admin", id = x.ID})
							   }
						   });

			return Json(res, JsonRequestBehavior.AllowGet);
		}

#endregion
	}
}