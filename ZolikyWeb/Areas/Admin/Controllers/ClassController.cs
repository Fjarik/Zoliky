using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
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
			var schoolId = this.User.Identity.GetSchoolId();

			bool active = onlyActive != null;

			var res = await Mgr.GetAllAsync(schoolId, active);
			if (!res.Any()) {
				this.AddErrorToastMessage("Nebyly nalezeny žádné třídy");
				return RedirectToApp();
			}
			var model = new ClassModel(res, active);
			return View(model);
		}

#region Create

#endregion

#region Edit & Detail

#endregion

#region Remove

#endregion
	}
}