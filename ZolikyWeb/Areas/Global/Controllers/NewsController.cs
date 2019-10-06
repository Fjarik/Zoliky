using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Global.Models.News;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper)]
	public class NewsController : OwnController<NewsManager>
	{
		public async Task<ActionResult> Dashboard()
		{
			var res = await Mgr.GetAllAsync();
			var model = new NewsDashboardModel(res);
			return View(model);
		}

#region Create

		public ActionResult Create()
		{
			var model = NewsModel.CreateModel();
			return View("Edit", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(NewsModel.ID))]
		public async Task<ActionResult> Create(NewsModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			if (!model.IsValid) {
				this.AddErrorToastMessage("Neplatné hodnoty");
				return RedirectToAction("Edit");
			}
			var loggedId = this.User.Identity.GetId();

			var res = await Mgr.CreateAsync(loggedId,
											model.Title,
											model.Message,
											expiration: model.Expiration);

			if (!res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se vytvořit záznam. Chyba: {res.GetStatusMessage()}");
				return RedirectToAction("Dashboard");
			}
			var original = res.Content;

			this.AddSuccessToastMessage("Novinka byla úspěšně vytvořena");
			return RedirectToAction("Detail", new {id = original.ID});
		}

#endregion

#region Edit & Detail

#region Edit

		public Task<ActionResult> Edit(int? id = null)
		{
			return EditOrDetail(id, "Edit", true);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(NewsModel.ID))]
		public async Task<ActionResult> Edit(NewsModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			if (!model.IsValid) {
				this.AddErrorToastMessage("Neplatné hodnoty");
				return RedirectToAction("Edit");
			}

			var res = await Mgr.GetByIdAsync(model.ID);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se načíst originální záznam. Chyba: {res.GetStatusMessage()}");
				return RedirectToAction("Dashboard");
			}


			var original = res.Content;

			// Edit
			original.Title = model.Title;
			original.Message = model.Message;
			original.Expiration = model.Expiration;
			// Edit end

			await Mgr.SaveAsync(original);
			this.AddSuccessToastMessage("Úspěšně uloženo");
			return RedirectToAction("Detail", new {id = model.ID});
		}

#endregion

#region Detail

		public Task<ActionResult> Detail(int? id = null)
		{
			return EditOrDetail(id, "Edit", false);
		}

#endregion

#region EditOrDetails

		private async Task<ActionResult> EditOrDetail(int? id, string actionName, bool allowEdit)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}
			return await EditOrDetail((int) id, actionName, allowEdit);
		}

		private async Task<ActionResult> EditOrDetail(int id, string actionName, bool allowEdit)
		{
			var res = await Mgr.GetByIdAsync(id);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Neplatná novinka");
				return RedirectToAction("Dashboard");
			}
			var previousId = await Mgr.GetPreviousIdAsync(id);
			var nextId = await Mgr.GetNextIdAsync(id);

			string url = Url.Action(actionName, "News", new {Area = "Global", id = UrlParameter.Optional});

			var model = new NewsModel(res.Content, allowEdit, previousId, nextId, url) {
				ActionName = actionName
			};

			return View("Edit", model);
		}

#endregion

#endregion

#region Remove

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(NewsModel.ID))]
		public Task<ActionResult> Remove(NewsModel model)
		{
			return RemoveAsync(model?.ID);
		}

		private async Task<ActionResult> RemoveAsync(int? id = null)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Neplatné ID");
				return RedirectToAction("Dashboard");
			}
			return await RemoveAsync((int) id);
		}

		private async Task<ActionResult> RemoveAsync(int id)
		{
			var res = await Mgr.DeleteAsync(id);
			if (res) {
				this.AddSuccessToastMessage("Novinka byla úspěšně odstraněna");
			} else {
				this.AddErrorToastMessage("Nezdařilo se odstranit novinku");
			}

			return RedirectToAction("Dashboard");
		}

#endregion
	}
}