using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Global.Models.Admin;
using ZolikyWeb.Areas.Global.Models.Admin.SettingsModels;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper)]
	public class SettingsController : OwnController
	{
		public async Task<ActionResult> Dashboard()
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
				return RedirectToAction("Dashboard");
			}
			var sMgr = this.GetManager<ProjectSettingManager>();

			foreach (var elm in dictionary) {
				var res = await sMgr.CreateAsync(null, elm.Key, elm.Value, true);
				if (!res.IsSuccess) {
					this.AddErrorToastMessage("Nezdařilo se uložit změny");
					return RedirectToAction("Dashboard");
				}
			}
			this.AddSuccessToastMessage("Nastavení bylo uloženo");
			return RedirectToAction("Dashboard");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<ActionResult> EditGlobalProjects(ProjectSettingsModel model)
		{
			return SaveSettings(model?.Dictionary);
		}
	}
}