using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Controllers
{
	[Authorize]
	public class TransactionsController : OwnController<TransactionManager>
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public async Task<ActionResult> Dashboard()
		{
			var id = this.User.Identity.GetId();
			var tester = this.User.Identity.IsTester();

			var res = await Mgr.UserTransactionsAsync(id, tester);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Nezdařilo se načíst pohyby");
				return RedirectToApp();
			}
			return View(res.Content);
		}
	}
}