using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Global.Models.User;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloper)]
	public class UserController : OwnController<UserManager>
	{
#region User list (Dashboard)

		public ActionResult Dashboard()
		{
			return View();
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
							   : Url.Action("Edit", "User", new {Area = "Global", id = x.ID}),
					display = Url.Action("Detail", "User", new {Area = "Global", id = x.ID})
				}
			});

			return Json(res, JsonRequestBehavior.AllowGet);
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
		[ValidateSecureHiddenInputs(nameof(UserModel.ID))]
		public async Task<ActionResult> Edit(UserModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			if (!model.IsValid) {
				this.AddErrorToastMessage("Neplatné hodnoty");
				return RedirectToAction("Edit");
			}

			var res = await Mgr.GetByIdAsync(model.ID);
			if (res.Status != StatusCode.NotEnabled && !res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se načíst originální záznam. Chyba: {res.GetStatusMessage()}");
				return RedirectToAction("Dashboard");
			}


			var original = res.Content;

			// Edit
			if (!original.IsInRolesOr(UserRoles.Teacher, UserRoles.SchoolManager) && model.ClassID > 0) {
				original.ClassID = model.ClassID;
			}
			original.SchoolID = model.SchoolID;
			original.Username = model.Username;
			original.Email = model.Email;
			original.Name = model.Name;
			original.Lastname = model.Lastname;
			original.Sex = model.Sex;
			original.Enabled = model.Enabled;
			if (!original.EmailConfirmed) {
				original.EmailConfirmed = model.EmailConfirmed;
			}
			// Edit end

			await Mgr.SaveAsync(original);

			// Roles edit
			res = await Mgr.SetRolesAsync(original.ID, model.RoleIds);
			// Roles edit end

			if (res.IsSuccess) {
				this.AddSuccessToastMessage("Úspěšně uloženo");
			} else {
				this.AddErrorToastMessage("Nezdařilo se upravit role");
			}
			return RedirectToAction("Detail", new {id = model.ID});
		}

#endregion

#region Detail

		public Task<ActionResult> Detail(int? id = null)
		{
			return EditOrDetail(id, "Edit", false);
		}

#endregion

#region Edit or Detail

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
			if (res.Status != StatusCode.NotEnabled && !res.IsSuccess) {
				this.AddErrorToastMessage("Neplatný uživatel");
				return RedirectToAction("Dashboard");
			}

			var loggedId = this.User.Identity.GetId();
			if (loggedId == id) {
				allowEdit = false;
			}

			var schoolId = this.User.GetSchoolId();

			var user = res.Content;

			if (!this.User.IsInRolesOr(UserRoles.SchoolManager, UserRoles.Administrator, UserRoles.Developer) &&
				user.IsInRole(UserRoles.SchoolManager)) {
				this.AddErrorToastMessage("Nemůžete upravovat svého nadřízeného");
				return RedirectToAction("Dashboard");
			}

			if (!this.User.IsInRolesOr(UserRoles.Administrator, UserRoles.Developer)) {
				if (user.SchoolID != schoolId || user.IsInRolesOr(UserRoles.Administrator, UserRoles.LoginOnly,
																  UserRoles.Public, UserRoles.HiddenStudent)) {
					this.AddErrorToastMessage("Nemáte oprávnění upravovat tohoto uživatele");
					return RedirectToAction("Dashboard");
				}
			}

			var previousId = await Mgr.GetPreviousStudentIdAsync(id, schoolId);
			var nextId = await Mgr.GetNextStudentIdAsync(id, schoolId);

			var sMgr = this.GetManager<SchoolManager>();
			var cMgr = this.GetManager<ClassManager>();

			var classes = await cMgr.GetAllAsync(schoolId, true);
			var schools = await sMgr.GetAllAsync();
			var roles = await Mgr.GetAllRolesAsync();

			var model = new UserModel(user, classes, schools, roles, allowEdit, previousId, nextId) {
				ActionName = actionName
			};

			return View("Edit", model);
		}

#endregion

#endregion

#region Create

		public async Task<ActionResult> Create()
		{
			var schoolId = this.User.GetSchoolId();

			var sMgr = this.GetManager<SchoolManager>();

			var classes = await sMgr.GetClassesAsync(schoolId);
			var schools = await sMgr.GetAllAsync();
			var roles = await Mgr.GetAllRolesAsync();

			var model = UserModel.CreateModel(classes, schools, roles);
			return View("Edit", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(UserModel.ID))]
		public async Task<ActionResult> Create(UserModel model)
		{
			if (model == null) {
				return RedirectToAction("Dashboard");
			}

			if (!model.IsValid) {
				this.AddErrorToastMessage("Neplatné hodnoty");
				return RedirectToAction("Edit");
			}

			var schoolId = this.User.GetSchoolId();
			var ip = this.Request.GetIPAddress();

			var res = await Mgr.StudentCreateAsync(model.Email, model.Password,
												   model.Name, model.Lastname,
												   model.Username, model.Sex,
												   model.ClassID, schoolId,
												   ip, model.Enabled);

			if (!res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se vytvořit záznam. Chyba: {res.GetStatusMessage()}");
				return RedirectToAction("Dashboard");
			}

			var original = res.Content;
			res = await Mgr.SetRolesAsync(original.ID, model.RoleIds);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se nastavit role");
			} else {
				this.AddSuccessToastMessage("Uživatel byl úspěšně vytvořen");
			}
			return RedirectToAction("Detail", new {id = original.ID});
		}

#endregion
	}
}