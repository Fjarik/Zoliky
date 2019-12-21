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
using ZolikyWeb.Models.Base;
using ZolikyWeb.Models.Home;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Controllers
{
	[OfflineActionFilter(Active = false)]
	public class HomeController : OwnController
	{
		[HttpGet]
		public ActionResult Index(ContactUsModel cModel = null)
		{
			var model = new HomeModel {
				StudentCount = 92,
				SchoolCount = 1,
				ZolikCount = 145,
				AchievementCount = 100,
				ContactUs = cModel ?? new ContactUsModel()
			};
			ModelState.Clear();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ContactUs(ContactUsModel model)
		{
			if (model == null) {
				model = new ContactUsModel();
			}
			if (!model.IsValid || !ModelState.IsValid) {
				this.AddErrorToastMessage("Nebyly zadány platné údaje");
				return RedirectToAction("Index");
			}
			if (!await IsRecaptchaValidAsync()) {
				this.AddErrorToastMessage("Musíte potvrdit, že nejste robot");
				return RedirectToAction("Index", model);
			}

			var eMgr = this.GetManager<MailManager>();
			var res = await eMgr.ContactUsAsync(model.Name, model.Email, model.Message);
			if (res) {
				this.AddSuccessToastMessage("Úspěšně odesláno");
			} else {
				this.AddErrorToastMessage("Nezdařilo se odeslat formulář. Zkuste to prosím později.");
			}
			return RedirectToAction("Index");
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
		[OutputCache(Duration = 120, VaryByParam = "none")]
		public ActionResult Status()
		{
			var res = new WebStatus(PageStatus.Functional);
			return Json(res, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[OutputCache(Duration = 120, VaryByParam = "none")]
		public ActionResult IsDown()
		{
			return Content(false.ToString());
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

		[HttpGet]
		public ActionResult PP()
		{
			return View();
		}

		[HttpGet]
		public ActionResult TOS()
		{
			return View();
		}
	}
}