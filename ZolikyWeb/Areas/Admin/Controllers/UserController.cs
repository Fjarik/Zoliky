using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Admin.Models.User;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper)]
	public class UserController : OwnController<UserManager>
	{
		public async Task<ActionResult> Dashboard()
		{
			var users = await this.Mgr.GetAllAsync();

			var model = new UserModel {
				Users = users.Where(x => !x.IsInRole(UserRoles.Robot))
			};
			return View(model);
		}

		public async Task<ActionResult> Edit(int? id = null)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Nejdříve vyberte uživatele");
				return RedirectToAction("Dashboard");
			}
			var userId = (int) id;

			return View();
		}
	}
}