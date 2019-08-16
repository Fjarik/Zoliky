using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using SharedLibrary;
using SharedLibrary.Enums;
using ZolikyWeb.Models.Home;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Controllers
{
	[OfflineActionFilter(Active = false)]
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			var model = new HomeModel {
				StudentCount = 92,
				SchoolCount = 1,
				ZolikCount = 145,
				AchievementCount = 100
			};
			return View(model);
		}

		[HttpGet]
		public ActionResult Mobile()
		{
			return RedirectToAction("Index", "Mobile", new {Area = ""});
		}

		[HttpGet]
		public ActionResult IndexOld()
		{
			return View();
		}
		

#region Project Status

		[HttpGet]
		public async Task<ActionResult> Offline()
		{
			var res = await GetProjectStatusAsync();
			var unv = new Unavailability() {
				Description = "Probíhá údržba webu, vydržte prosím",
				Reason = "Údržba",
				From = DateTime.Now.AddMinutes(-5),
				To = DateTime.Now.AddMinutes(5),
				ProjectID = (int) Projects.WebNew
			};
			if (res.Content is Unavailability u) {
				unv = u;
			}

			return View(unv);
		}

		[HttpGet]
		[OutputCache(Duration = 120, VaryByParam = "none")]
		public async Task<ActionResult> Status()
		{
			var res = await GetProjectStatusAsync();
			return Json(res, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[OutputCache(Duration = 120, VaryByParam = "none")]
		public async Task<ActionResult> IsDown()
		{
			var mgr = this.GetManager<UnavailabilityManager>();
			var isUnavaible = await mgr.IsUnavailableAsync(Projects.Api);
			if (!isUnavaible) {
				return Content(false.ToString());
			}

			isUnavaible = await mgr.IsUnavailableAsync(Globals.Project);
			return Content(isUnavaible.ToString());
		}

		private async Task<WebStatus> GetProjectStatusAsync()
		{
			var res = await GetProjectStatusAsync(Projects.Api);
			if (!res.CanAccess) {
				return res;
			}

			res = await GetProjectStatusAsync(Globals.Project);
			return res;
		}

		private async Task<WebStatus> GetProjectStatusAsync(Projects project)
		{
			var mgr = this.GetManager<UnavailabilityManager>();
			var isUnavaible = await mgr.IsUnavailableAsync(project);
			if (!isUnavaible) {
				return new WebStatus(PageStatus.Functional);
			}

			var res = mgr.GetFirst(project);
			if (!res.IsSuccess) {
				return new WebStatus(PageStatus.Limited, "Vyskytla se chyba při zjištování dostupnosti");
			}

			var unvs = res.Content;
			if (unvs == null) {
				return new WebStatus(PageStatus.Limited, "Vyskytla se chyba při zjištování dostupnosti");
			}

			return new WebStatus(PageStatus.NotAvailable, null, unvs);
		}

#endregion

#region Account routes

		[HttpGet]
		public ActionResult Login()
		{
			return RedirectToAction("Login", "Account", new {Area = ""});
		}

		[HttpGet]
		public ActionResult Register()
		{
			return RedirectToAction("Register", "Account", new {Area = ""});
		}

		[HttpGet]
		public ActionResult ForgetPwd()
		{
			return RedirectToAction("ForgotPassword", "Account", new {Area = ""});
		}

#endregion
	}
}