using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;
using ZolikyWeb.Areas.App.Models.Zoliky;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Controllers
{
	[Authorize]
	public class ZolikyController : OwnController<ZolikManager>
	{
		[HttpGet]
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		[HttpGet]
		public async Task<ActionResult> Dashboard()
		{
			var zoliks = await this.GetUserZoliksAsync();
			return View(zoliks);
		}

#region Lock & Unlock

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ZolikLockModel.ZolikId))]
		public async Task<ActionResult> Lock(ZolikLockModel model)
		{
			if (model == null || !ModelState.IsValid || !model.IsValid) {
				this.AddErrorToastMessage("Neplatné vstupní údaje");
				return RedirectToAction("Dashboard");
			}
			var logged = await this.GetLoggedUserAsync();
			if (logged == null) {
				return RedirectToLogin();
			}

			var res = await Mgr.LockAsync(model, logged);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage(res.GetStatusMessage());
			} else {
				this.AddSuccessToastMessage("Žolík byl úspěšně uzamčen.");
			}
			return RedirectToAction("Dashboard");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ZolikLock.ZolikId))]
		public async Task<ActionResult> Unlock(ZolikLock model)
		{
			if (model == null || !ModelState.IsValid || model.ZolikId < 1) {
				this.AddErrorToastMessage("Neplatné vstupní údaje");
				return RedirectToAction("Dashboard");
			}
			var logged = await this.GetLoggedUserAsync();
			if (logged == null) {
				return RedirectToLogin();
			}

			var res = await Mgr.UnlockAsync(model.ZolikId, logged);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage(res.GetStatusMessage());
			} else {
				this.AddSuccessToastMessage("Žolík byl úspěšně odemčen.");
			}
			return RedirectToAction("Dashboard");
		}

#endregion

#region Transfer

		[HttpGet]
		public async Task<ActionResult> Transfer(int? id = null)
		{
			if (User.IsInRole(UserRoles.Public)) {
				this.AddErrorToastMessage("Na tuto stránku nemáte přístup");
				return RedirectToApp();
			}

			var zoliks = (await this.GetUserZoliksAsync());
			zoliks = zoliks.Where(x => x.CanBeTransfered).ToList();
			var model = new ZolikTransferModel() {
				Zoliks = zoliks
			};
			if (id != null && zoliks.Any(x => x.ID == id)) {
				model.ZolikID = (int) id;
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Transfer(ZolikTransferModel model)
		{
			if (model == null) {
				model = new ZolikTransferModel();
			}
			var logged = await this.GetLoggedUserAsync();
			if (logged == null) {
				return RedirectToLogin();
			}
			if (ModelState.IsValid && model.IsValid) {
				var pack = new ZolikPackage {
					Type = TransactionAssignment.Gift,
					ZolikID = model.ZolikID,
					FromID = logged.ID,
					ToID = model.ToID,
					Message = model.Message,
				};
				var res = await Mgr.TransferAsync(pack, logged);
				var error = res.GetStatusMessage();
				switch (res.Status) {
					case StatusCode.NotValidID:
						error = "Vybrané objekty nejsou platné";
						break;
					case StatusCode.JustALittleError:
						error = "Vyskytla se pouze menší chyba (žolík se nejspíše odeslal, ale XP se nepřipsaly)";
						break;
				}
				if (res.IsSuccess) {
					this.AddSuccessToastMessage("Žolík byl úspěšně darován");
					return RedirectToAction("Dashboard");
				}
				this.AddErrorToastMessage(error);
			}
			var zoliks = await this.GetUserZoliksAsync();
			zoliks = zoliks.Where(x => x.CanBeTransfered).ToList();
			model.Zoliks = zoliks;
			this.AddErrorToastMessage("Vyskytla se chyba");
			return View(model);
		}

#endregion

#region Split

		[HttpGet]
		public async Task<ActionResult> Split(int? id = null)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Musíte vybrat žolíka k rozdělení");
				return RedirectToAction("Dashboard");
			}
			var res = await this.Mgr.GetByIdAsync((int) id);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Nezdařilo se načíst daného žolíka");
				return RedirectToAction("Dashboard");
			}
			var zolik = res.Content;

			var userId = this.User.Identity.GetId();

			if (zolik.OwnerID != userId) {
				this.AddErrorToastMessage("Tento žolík není Váš");
				return RedirectToAction("Dashboard");
			}
			if (!zolik.IsSplittable) {
				this.AddErrorToastMessage("Tohoto žolíka nelze rozdělit");
				return RedirectToAction("Dashboard");
			}
			return View(zolik);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(Zolik.ID))]
		public async Task<ActionResult> Split(Zolik model)
		{
			if (model == null || model.ID < 1) {
				this.AddErrorToastMessage("Neplatný žolík");
				return RedirectToAction("Dashboard");
			}
			var user = await this.GetLoggedUserAsync();
			if (user == null) {
				this.AddErrorToastMessage("Musíte být přihlášení");
				return RedirectToLogin();
			}
			var res = await Mgr.SplitAsync(model.ID, user);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage(res.GetStatusMessage());
				return RedirectToAction("Dashboard");
			}
			var content = res.Content;
			var count = content.Count - 1;
			var text = "žolíky";
			if (count > 4) {
				text = "žolíků";
			}
			this.AddSuccessToastMessage($"Zdařilo se. Váš jokér byl rozdělen na {count} {text}");
			return RedirectToAction("Dashboard");
		}

#endregion
	}
}