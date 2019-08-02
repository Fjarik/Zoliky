using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary.Shared;
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

		public ActionResult Create()
		{
			return View();
		}

		public ActionResult Edit()
		{
			return View();
		}
	}
}