using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Managers.New.Interfaces;
using DataAccess.Models;
using hbehr.recaptcha;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Models.Base
{
	public class OwnController : Controller
	{
		protected ActionResult RedirectToLocal(string r, string actionName = "Index", string controllerName = "Home")
		{
			if (Url.IsLocalUrl(r)) {
				return Redirect(r);
			}
			return RedirectToAction(actionName, controllerName);
		}

		protected Task<bool> IsRecaptchaValidAsync()
		{
			string userResponse = HttpContext.Request.Params["g-recaptcha-response"];
			return ReCaptcha.ValidateCaptchaAsync(userResponse);
		}

		protected ActionResult RedirectToApp(string r = null)
		{
			if (r != null && Url.IsLocalUrl(r)) {
				return Redirect(r);
			}
			return RedirectToAction("Dashboard", "Main", new {Area = "App"});
		}

		protected ActionResult RedirectToLogin(string r = null)
		{
			if (r != null && Url.IsLocalUrl(r)) {
				return Redirect(r);
			}
			return RedirectToAction("Login", "Account", new {Area = ""});
		}
	}

	public class OwnController<TManager> : OwnController where TManager : class, IManager, IDisposable
	{
		private TManager _mgr;

		protected TManager Mgr => _mgr ?? (_mgr = this.GetManager<TManager>());

		protected async Task<IList<Zolik>> GetUserZoliksAsync()
		{
			var mgr = this.GetManager<ZolikManager>();
			if (!this.IsAuthenticated()) {
				return new List<Zolik>();
			}
			var userId = this.User.Identity.GetId();
			if (userId < 1) {
				return new List<Zolik>();
			}
			var isTester = this.User.Identity.IsTester();
			var res = await mgr.GetUsersZoliksAsync(userId, isTester);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Nezdařilo se získat žolíky");
				return new List<Zolik>();
			}
			return res.Content;
		}
	}
}