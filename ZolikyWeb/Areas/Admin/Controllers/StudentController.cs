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
using ZolikyWeb.Areas.Admin.Models.Student;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Controllers
{
	[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
	public class StudentController : OwnController<UserManager>
	{
#region Student list (Dashboard)

		public ActionResult Dashboard()
		{
			return View();
		}

		public async Task<JsonResult> UsersJson()
		{
			var schoolId = this.User.GetSchoolId();
			var loggedId = this.User.Identity.GetId();

			var sMgr = this.GetManager<SchoolManager>();

			var users = await sMgr.GetStudentsAsync(schoolId);


			var res = users.OrderBy(x => x.ClassName)
						   .ThenBy(x => x.Lastname)
						   .ThenBy(x => x.Name)
						   .ThenBy(x => x.Name)
						   .Select(x => new {
							   id = x.ID,
							   fullname = x.FullName,
							   email = x.Email,
							   @class = new {
								   name = x.Class.Name,
								   since = x.Class.Since.Year,
								   grad = x.Class.Graduation.Year
							   },
							   zolikCount = x.OriginalZoliks.Count(y => !y.Type.IsTesterType() && y.Enabled),
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
								   edit = x.ID == loggedId
											  ? ""
											  : Url.Action("Edit", "Student", new {Area = "Admin", id = x.ID}),
								   display = Url.Action("Detail", "Student", new {Area = "Admin", id = x.ID})
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
		[ValidateSecureHiddenInputs(nameof(StudentModel.ID))]
		public async Task<ActionResult> Edit(StudentModel model)
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
			original.ClassID = model.ClassID;
			//original.SchoolID = model.SchoolID;
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
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Neplatný uživatel");
				return RedirectToAction("Dashboard");
			}
			var loggedId = this.User.Identity.GetId();
			if (loggedId == id) {
				allowEdit = false;
			}

			var schoolId = this.User.GetSchoolId();
			var previousId = await Mgr.GetPreviousIdAsync(id);
			var nextId = await Mgr.GetNextIdAsync(id);

			var sMgr = this.GetManager<SchoolManager>();
			var cMgr = this.GetManager<ClassManager>();

			var classes = await cMgr.GetAllAsync(schoolId, true);

			var sRes = await sMgr.GetByIdAsync(schoolId);
			if (!sRes.IsSuccess) {
				this.AddErrorToastMessage("Neplatná škola");
				return RedirectToAction("Dashboard");
			}

			var schools = new List<School> {sRes.Content};

			string url = Url.Action(actionName, "Student", new {Area = "Admin", id = UrlParameter.Optional});

			var model = new StudentModel(res.Content, classes, schools, allowEdit, previousId, nextId, url) {
				ActionName = actionName
			};

			return View("Edit", model);
		}

#endregion

#endregion

#region Create

		public ActionResult Create()
		{
			this.AddErrorToastMessage("Každý student se musí zaregistovat sám za sebe");
			return RedirectToAction("Dashboard");
		}

#endregion
	}
}