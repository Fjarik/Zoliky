﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Models;
using FileHelpers;
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
			if (res.Status != StatusCode.NotEnabled && !res.IsSuccess) {
				this.AddErrorToastMessage($"Nezdařilo se načíst originální záznam. Chyba: {res.GetStatusMessage()}");
				return RedirectToAction("Dashboard");
			}


			var original = res.Content;

			// Edit
			if (!original.IsInRolesOr(UserRoles.Teacher, UserRoles.SchoolManager)) {
				original.ClassID = model.ClassID;
			}
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

			var sRes = await sMgr.GetByIdAsync(schoolId);
			if (!sRes.IsSuccess) {
				this.AddErrorToastMessage("Neplatná škola");
				return RedirectToAction("Dashboard");
			}

			var schools = new List<School> {sRes.Content};

			var model = new StudentModel(user, classes, schools, allowEdit, previousId, nextId) {
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
			// var schools = await this.GetSchoolAsync();

			// Pouze škola registrovaného správce školy - Nemůže přidat třídu do cizí školy
			var sMgr = this.GetManager<SchoolManager>();
			var res = await sMgr.GetByIdAsync(schoolId);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Nezdařilo se nalézt Vaši školu");
				return RedirectToAction("Dashboard");
			}
			var classes = await sMgr.GetClassesAsync(schoolId);

			var schools = new List<School> {
				res.Content
			};

			var model = StudentModel.CreateModel(classes, schools);
			return View("Edit", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(StudentModel.ID))]
		public async Task<ActionResult> Create(StudentModel model)
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

			this.AddSuccessToastMessage("Student byl úspěšně vytvořen");
			return RedirectToAction("Detail", new {id = original.ID});
		}

#endregion

#region Remove

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(StudentModel.ID))]
		public Task<ActionResult> Remove(StudentModel model)
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
				this.AddSuccessToastMessage("Student úspěšně odstraněn");
			} else {
				this.AddErrorToastMessage("Nezdařilo se odstranit studenta");
			}

			return RedirectToAction("Dashboard");
		}

#endregion

#region Down/Upgrade

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(StudentModel.ID))]
		public async Task<ActionResult> Upgrade(StudentModel model)
		{
			var uMgr = this.GetManager<UserManager>();

			var res = await uMgr.AddToRoleAsync(model.ID, UserRoles.Teacher);
			if (res.IsSuccess) {
				res = await uMgr.RemoveFromRoleAsync(model.ID, UserRoles.Student);
			}

			if (res.IsSuccess) {
				this.AddSuccessToastMessage("Uživatel byl úspěšně povýšen");
			} else {
				this.AddErrorToastMessage("Nezdařilo se povýšit uživatele");
			}
			return RedirectToAction("Dashboard");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(StudentModel.ID))]
		public async Task<ActionResult> Downgrade(StudentModel model)
		{
			var uMgr = this.GetManager<UserManager>();

			var res = await uMgr.AddToRoleAsync(model.ID, UserRoles.Student);
			if (res.IsSuccess) {
				res = await uMgr.RemoveFromRoleAsync(model.ID, UserRoles.Teacher);
			}

			if (res.IsSuccess) {
				this.AddSuccessToastMessage("Uživatel byl úspěšně degradován");
			} else {
				this.AddErrorToastMessage("Nezdařilo se degradovat uživatele");
			}
			return RedirectToAction("Dashboard");
		}

#endregion

#region Import

		public ActionResult Import()
		{
			var model = new ImportStudentsModel {
				HasHeader = true
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UploadFile(ImportStudentsModel model)
		{
			if (model == null) {
				model = new ImportStudentsModel();
			}
			if (model.File == null && Request.Files.Count > 0) {
				model.File = Request.Files[0];
			}


			var file = model.File;

			if (file == null) {
				return View("Import", model);
			}

			byte[] bytes;
			using (var br = new BinaryReader(file.InputStream)) {
				bytes = br.ReadBytes((int) file.InputStream.Length);
			}

			var folder = Server.MapPath("~/Content/temp_csv/");

			var g = Guid.NewGuid().ToString("N");

			var filename = $"{g.Substring(0, 10)}.csv";

			var path = WriteFile(folder, filename, bytes);

			var students = ReadFile(path, model.HasHeader);

			RemoveFile(path);

			model.Step2 = new ImportStudentModel2(students);

			return View("Import", model);
		}

		private List<ImportStudent> ReadFile(string path, bool hasHeader)
		{
			var engine = new FileHelperAsyncEngine<ImportStudent>();

			var students = new List<ImportStudent>();

			using (engine.BeginReadFile(path)) {
				if (hasHeader) {
					foreach (var student in engine.Skip(1)) {
						students.Add(student);
					}
				} else {
					foreach (var student in engine) {
						students.Add(student);
					}
				}
			}
			return students;
		}

		private string WriteFile(string folder, string fileName, byte[] bytes)
		{
			if (!Directory.Exists(folder)) {
				var path = new Uri(folder).LocalPath;

				Directory.CreateDirectory(path);
			}

			var full = Path.Combine(folder, fileName);

			System.IO.File.WriteAllBytes(full, bytes);

			return full;
		}

		private void RemoveFile(string path)
		{
			if (System.IO.File.Exists(path)) {
				System.IO.File.Delete(path);
			}
		}

#endregion
	}
}