using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Global.Models.Ban;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper)]
	public class BanController : OwnController<BanManager>
	{
		public async Task<ActionResult> Dashboard()
		{
			var bans = await Mgr.GetActiveBansAsync();
			if (!bans.Any()) {
				this.AddErrorToastMessage("Žádný uživatel není aktulně zablokovaný");
				return this.RedirectToApp();
			}
			return View(bans);
		}

		public async Task<ActionResult> Edit(int? id = null)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}
			var res = await Mgr.GetByIdAsync((int) id);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Neplatný ban");
				return RedirectToAction("Dashboard");
			}

			var model = new BanModel(res.Content);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(BanModel.ID))]
		public async Task<ActionResult> Edit(BanModel b)
		{
			if (b == null || !b.IsValid) {
				this.AddErrorToastMessage("Neplatné změny");
				return RedirectToAction("Dashboard");
			}

			var res = await Mgr.GetByIdAsync(b.ID);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Neplatný ban");
				return RedirectToAction("Dashboard");
			}

			var original = res.Content;
			original.Reason = b.Reason;
			original.To = b.To;

			var suc = await Mgr.SaveAsync(original) == 1;
			if (!suc) {
				this.AddErrorToastMessage("Nezdařilo se u");
			}
			return RedirectToAction("Dashboard");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(Ban.ID))]
		public async Task<ActionResult> Unban(Ban b)
		{
			if (b == null || b.ID < 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}

			var res = await Mgr.UnbanAsync(b.UserID);
			if (!res) {
				this.AddErrorToastMessage("Nezdařilo se odblokovat uživatele");
			}
			return RedirectToAction("Dashboard");
		}
	}
}