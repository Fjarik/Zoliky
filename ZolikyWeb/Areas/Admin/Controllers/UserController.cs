using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using DataAccess.Models;
using Newtonsoft.Json;
using SharedLibrary.Enums;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Admin.Models.User;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper)]
	public class UserController : OwnController<UserManager>
	{
		public ActionResult Dashboard()
		{
			return View();
		}

		public async Task<ActionResult> Edit(int? id = null)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Nejdříve vyberte uživatele");
				return RedirectToAction("Dashboard");
			}
			var userId = (int) id;
			var user = await this.Mgr.GetByIdAsync(userId);

			return View(user);
		}

		public async Task<ActionResult> Detail(int? id = null)
		{
			if (id == null || id < 1) {
				this.AddErrorToastMessage("Nejdříve vyberte uživatele");
				return RedirectToAction("Dashboard");
			}
			var userId = (int) id;
			var user = await this.Mgr.GetByIdAsync(userId);

			return View(user);
		}

		public async Task<JsonResult> UsersJson()
		{
			var users = (await this.Mgr.GetAllAsync()).Where(x => !x.IsInRolesOr(UserRoles.Robot, UserRoles.LoginOnly));

			var res = users.Select(x => new {
				//options = new { },
				id = x.ID,
				fullname = x.FullName,
				email = x.Email,
				@class = x.Class == null
							 ? null
							 : new {
								 name = x.Class.Name,
								 since = x.Class.Since.Year,
								 grad = x.Class.Graduation.Year
							 },
				roles = x.Roles
						 .Where(y => y.Name != UserRoles.Support)
						 .Select(y => new {
							 name = y.FriendlyName,
							 desc = y.Description,
							 color = y.GetColor()
						 }),
				memberSince = new {
					date = x.MemberSince.ToString("dd.MM.yyyy"),
					timeS = x.MemberSince.GetJsTimestamp(),
					regIp = x.RegistrationIp
				},
				lastLogin = x.LastLogin == null
								? new {
									date = "",
									timeS = 0.0,
									project = ""
								}
								: new {
									date = x.LastLogin.Date.ToString("dd.MM.yyyy"),
									timeS = x.LastLogin.Date.GetJsTimestamp(),
									project = ((Projects) x.LastLogin.ProjectID).GetDescription()
								},
				x.IsBanned,
				x.IsEnabled,
				actions = new {
					edit = x.IsInRolesOr(UserRoles.Administrator, UserRoles.Developer)
							   ? ""
							   : Url.Action("Edit", "User", new {Area = "Admin", id = x.ID}),
					display = Url.Action("Detail", "User", new {Area = "Admin", id = x.ID})
				}
			});

			return Json(res, JsonRequestBehavior.AllowGet);
		}
	}
}