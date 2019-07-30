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
using ZolikyWeb.Areas.Admin.Models.Admin.SettingsModels;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper + "," + UserRoles.Teacher)]
	public class AdminController : OwnController
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public ActionResult Dashboard()
		{
			var model = new DashboardModel() {
				SendNot = new SendNotificationModel() {
					ToId = 2
				},
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendNotification(SendNotificationModel model)
		{
			var fMgr = this.GetManager<FirebaseManager>();

			var id = User.Identity.GetId();
			var res = await fMgr.NewZolikAsync(model.ZolikId, id, model.ToId);
			if (res) {
				this.AddSuccessToastMessage("Úspěch");
			}

			return RedirectToAction("Dashboard");
		}

#region Settings

		[Authorize(Roles = UserRoles.AdminOrDeveloper)]
		public async Task<ActionResult> Settings()
		{
			var sMgr = this.GetManager<ProjectSettingManager>();
			var settings = await sMgr.GetAllAsync();

			var model = new SettingsModel(settings);
			return View(model);
		}

		private async Task<ActionResult> SaveSettings(IDictionary<string, bool> dictionary)
		{
			if (!this.IsAuthenticated()) {
				return RedirectToLogin();
			}

			if (dictionary == null) {
				this.AddErrorToastMessage("Nezdařilo se uložit změny");
				return RedirectToAction("Settings");
			}
			var sMgr = this.GetManager<ProjectSettingManager>();

			foreach (var elm in dictionary) {
				var res = await sMgr.CreateAsync(null, elm.Key, elm.Value, true);
				if (!res.IsSuccess) {
					this.AddErrorToastMessage("Nezdařilo se uložit změny");
					return RedirectToAction("Settings");
				}
			}
			this.AddSuccessToastMessage("Nastavení bylo uloženo");
			return RedirectToAction("Settings");
		}

		[Authorize(Roles = UserRoles.AdminOrDeveloper)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<ActionResult> EditProjects(ProjectSettingsModel model)
		{
			return SaveSettings(model?.Dictionary);
		}

#endregion
	}
}