using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Global.Models.Admin;
using ZolikyWeb.Areas.Global.Models.Admin.Dashboard;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper)]
	public class AdminController : OwnController<SchoolManager>
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public async Task<ActionResult> Dashboard()
		{
			var zoliks = await Mgr.GetZolikCountAsync();
			var students = await Mgr.GetStudentCountAsync();
			var teachers = await Mgr.GetTeacherCountAsync();
			var schools = await Mgr.GetSchoolCountAsync();


			var model = new DashboardModel(2) {
				ZoliksCount = zoliks,
				StudentsCount = students,
				TeachersCount = teachers,
				SchoolsCount = schools
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendMobileNotification(SendMobileNotModel model)
		{
			var fMgr = this.GetManager<FirebaseManager>();

			var id = User.Identity.GetId();
			var res = await fMgr.NewZolikAsync(model.ZolikId, id, model.ToId);
			if (res) {
				this.AddSuccessToastMessage("Úspěch");
			}

			return RedirectToAction("Dashboard");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendNotifications(SendNotificationsModel model)
		{
			if (model != null && model.IsValid) {
				var nMgr = this.GetManager<NotificationManager>();

				var res = await nMgr.CreateAsync(model.ToID,
												 model.Title,
												 model.Subtitle);
				if (!res.IsSuccess) {
					this.AddErrorToastMessage("Něco se nepovedlo");
				}
			}

			return RedirectToAction("Dashboard");
		}
	}
}