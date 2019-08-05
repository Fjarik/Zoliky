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
	public class AdminController : OwnController
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public ActionResult Dashboard()
		{
			var model = new DashboardModel();
			return View(model);
		}
	}
}