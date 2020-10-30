using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using hbehr.recaptcha;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;
using ZolikyWeb.Models.Account;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Controllers
{
	[AllowAnonymous]
	public class AccountController : OwnController<UserManager>
	{
#region Fields and Properties

		private SignInManager _signInManager;

		private SignInManager SignInManager
		{
			get => _signInManager ?? this.GetManager<SignInManager>();
			set => _signInManager = value;
		}

#endregion

#region Actions

#region Basic (Index, Access Denied...)

		public ActionResult Index()
		{
			return RedirectToAction("Login");
		}

		[HttpGet]
		public ActionResult AccessDenied()
		{
			this.AddErrorToastMessage("Na tuto akci nemáte dostatečné oprávnění");
			return RedirectToAction("Index", "Home");
		}

#endregion

#region Login & Logout

#region Normal

		[HttpGet]
		[OfflineActionFilter(Active = false)]
		public ActionResult Login(string r,
								  bool showActivationElement = false,
								  bool showRegistrationErrorElement = false)
		{
			if (this.IsAuthenticated()) {
				return RedirectToApp(r);
			}
			ViewBag.r = r;
			var lg = new LoginPageModel() {
				ShowActivationElement = showActivationElement,
				ShowRegistrationErrorElement = showRegistrationErrorElement
			};
			return View(lg);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[OfflineActionFilter(Active = false)]
		public async Task<ActionResult> Login(LoginPageModel lg, string r)
		{
			if (!ModelState.IsValid || lg == null || !lg.IsValid) {
				if (lg == null) {
					lg = new LoginPageModel();
				}
				this.AddErrorToastMessage("Neplatné vstupné údaje");
				return View(lg);
			}
			if (!await IsRecaptchaValidAsync()) {
				this.AddErrorToastMessage("Údajně jste robot!");
				return View(lg);
			}
			var logins = new Logins() {
				UName = lg.UName,
				Password = lg.Password,
				Project = Projects.WebNew,
			};

			var res = await Mgr.LoginAsync(logins, Request.GetIPAddress());

			string msg = res.GetStatusMessage();
			switch (res.Status) {
				case StatusCode.InvalidInput:
					msg = "Zadané údaje nejsou platné";
					break;
				case StatusCode.InsufficientPermissions:
					msg = "K přihlášení nemáte dostatečné oprávnění";
					break;
				case StatusCode.NotFound:
				case StatusCode.WrongPassword:
					msg = "Nesprávné jméno nebo heslo";
					break;
				case StatusCode.ExpiredPassword:
					msg = "Vašemu heslu vypršela platnost";
					break;
				case StatusCode.NotEnabled:
					lg.ShowAccountDisabledElement = true;
					break;
				case StatusCode.EmailNotConfirmed:
					lg.ShowActivationElement = true;
					break;
				case StatusCode.Banned:
					var banMgr = this.GetManager<BanManager>();
					var ban = await banMgr.GetActiveBanAsync(res.Content.ID);
					msg = $"Váš účet byl zablokován z důvodu: {ban.Reason}";
					break;
				case StatusCode.OK:
					if (res.Content is User u) {
						this.SignInManager.SignIn(u, lg.RememberMe);
						this.AddSuccessToastMessage("Přihlášení proběhlo úspěšně");
						if (u.IsInRolesOr(UserRoles.Administrator, UserRoles.Teacher)) {
							return RedirectToAction("Dashboard", "Admin", new {Area = "Admin"});
						}
						return this.RedirectToApp(r);
					}
					break;
				case StatusCode.InternalError:
					msg = "Omlouváme se, vyskytla se chyba na straně serveru. Zkuste to prosím později";
					break;
			}
			if (lg.ShowMessage) {
				ModelState.AddModelError("Message", msg);
			}
			ViewBag.r = r;
			lg.ClearPassword();
			return View(lg);
		}

#endregion

#region External

		[HttpPost]
		[ValidateAntiForgeryToken]
		[OfflineActionFilter(Active = false)]
		public ActionResult ExternalLogin(string provider, string r)
		{
			ControllerContext?.HttpContext?.Session?.RemoveAll();

			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new {r}));
		}

		[HttpGet]
		[OfflineActionFilter(Active = false)]
		public async Task<ActionResult> ExternalLoginCallback(string r)
		{
			var loginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
			if (loginInfo == null) {
				this.AddErrorToastMessage("Nebyly vráceny žádné informace. Zkuste to prosím znovu");
				return RedirectToLogin(r);
			}
			var res = await Mgr.LoginAsync(loginInfo, Request.GetIPAddress(), Projects.WebNew);
			string msg = res.GetStatusMessage();
			var lg = new LoginPageModel();
			switch (res.Status) {
				// NoPassword - default message
				case StatusCode.InvalidInput:
					msg = "Předané údaje nejsou platné";
					break;
				case StatusCode.InsufficientPermissions:
					msg = "K přihlášení nemáte dostatečné oprávnění";
					break;
				case StatusCode.NotFound:
					var exists = await Mgr.ExistsAsync(loginInfo.Email, WhatToCheck.Email);
					if (!exists) {
						return RedirectToRegister(loginInfo);
					}
					msg = $"Tento {loginInfo.Login.LoginProvider} účet není připojen k účtu žolíků";
					break;
				case StatusCode.WrongPassword:
					msg = "Nesprávné jméno nebo heslo";
					break;
				case StatusCode.ExpiredPassword:
					msg = "Vašemu heslu vypršela platnost";
					break;
				case StatusCode.Banned:
					var banMgr = this.GetManager<BanManager>();
					var ban = await banMgr.GetActiveBanAsync(res.Content.ID);
					msg = $"Váš účet byl zablokován z důvodu: {ban.Reason}";
					break;
				case StatusCode.NotEnabled:
					lg.ShowAccountDisabledElement = true;
					break;
				case StatusCode.EmailNotConfirmed:
					lg.ShowActivationElement = true;
					break;
				case StatusCode.OK:
					if (res.Content is User u) {
						SignInManager.SignIn(u, false);
						this.AddSuccessToastMessage("Přihlášení proběhlo úspěšně");
						if (u.IsInRolesOr(UserRoles.Administrator, UserRoles.Teacher)) {
							return RedirectToAction("Dashboard", "Admin", new {Area = "Admin"});
						}
						return RedirectToApp(r);
					}
					break;
				case StatusCode.InternalError:
					msg = "Omlouváme se, vyskytla se chyba na straně serveru. Zkuste to prosím později";
					break;
			}
			if (lg.ShowMessage) {
				ModelState.AddModelError("Message", msg);
			}
			ViewBag.r = r;
			this.AddErrorToastMessage(msg);
			return View("Login", lg);
		}

		private ActionResult RedirectToRegister(ExternalLoginInfo info)
		{
			var email = info.Email;
			var name = "";
			var lastname = "";
			if (info.ExternalIdentity.HasClaim(x => x.Type == ClaimTypes.Name)) {
				var fullName = info.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
				if (!string.IsNullOrWhiteSpace(fullName)) {
					var names = fullName.Split(' ');
					name = names.First();
					lastname = names.Last();
				}
			}
			var providerKey = info.Login.ProviderKey;
			var provider = info.Login.LoginProvider;

			return RedirectToAction("Register", new {email, name, lastname, providerKey, provider});
		}

#endregion

#region Logout

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[OfflineActionFilter(Active = false)]
		public ActionResult Logout(string r)
		{
			SignInManager.SignOut();
			this.AddSuccessToastMessage("Odhlášení proběhlo úspěšně");
			if (Url.IsLocalUrl(r)) {
				return Redirect(r);
			}
			return RedirectToAction("Login");
		}

#endregion

#endregion

#region Registration

		[HttpGet]
		public async Task<ActionResult> Register(string email = null, string name = null, string lastname = null,
												 string providerKey = null, string provider = null)
		{
			var sMgr = this.GetManager<ProjectSettingManager>();
			var allow = await sMgr.GetBoolAsync(null, ProjectSettingKeys.RegistrationEnabled);
			if (!allow) {
				this.AddErrorToastMessage("Nové registrace nejsou aktuálně dovoleny");
				return RedirectToLogin();
			}

			var schoolMgr = this.GetManager<SchoolManager>();

			var schools = await schoolMgr.GetAllAsync();
			var model = new RegisterModel {
				Schools = schools
			};
			if (!string.IsNullOrWhiteSpace(email)) {
				model.Email = email;
			}
			if (!string.IsNullOrWhiteSpace(name)) {
				model.Firstname = name;
			}
			if (!string.IsNullOrWhiteSpace(lastname)) {
				model.Lastname = lastname;
			}
			if (!Methods.AreNullOrWhiteSpace(providerKey, provider)) {
				model.ProviderKey = providerKey;
				model.Provider = provider;
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterModel m)
		{
			if (m == null) {
				m = new RegisterModel();
			}
			//var cMgr = this.GetManager<ClassManager>();
			var schoolMgr = this.GetManager<SchoolManager>();

			//m.Classes = await cMgr.GetAllAsync();
			m.Schools = await schoolMgr.GetAllAsync();
			if (!ModelState.IsValid) {
				m.ClearPasswords();
				this.AddErrorToastMessage("Nebyly zadány platné údaje");
				return View(m);
			}
			if (!await IsRecaptchaValidAsync()) {
				m.ClearPasswords();
				this.AddErrorToastMessage("Údajně jste robot!");
				return View(m);
			}

			if (!m.IsValid) {
				m.ClearPasswords();
				this.AddErrorToastMessage("Nebyly zadány platné údaje");
				return View(m);
			}
			int? classId = m.ClassId;
			if (classId == 0) {
				classId = null;
			}
			var url = Url.Action("Activate", "Account", new {Area = ""}, "https");
			MActionResult<User> res;
			if (m.IsExternal) {
				res = await Mgr.RegisterExternalAsync(m.Email,
													  m.ProviderKey,
													  m.Provider,
													  m.Firstname,
													  m.Lastname,
													  m.Username,
													  (byte) m.Gender,
													  classId,
													  m.SchoolId,
													  m.Newsletter,
													  m.FutureNews,
													  this.Request.GetIPAddress(),
													  url);
			} else {
				res = await Mgr.RegisterAsync(m.Email,
											  m.Password,
											  m.Firstname,
											  m.Lastname,
											  m.Username,
											  (byte) m.Gender,
											  classId,
											  m.SchoolId,
											  m.Newsletter,
											  m.FutureNews,
											  this.Request.GetIPAddress(),
											  url);
			}
			var msg = res.GetStatusMessage();
			switch (res.Status) {
				case StatusCode.InvalidInput:
					msg = "Zadané údaje nejsou platné";
					break;
				case StatusCode.WrongPassword:
					msg = "Heslo nemá správný formát. Heslo musí být dlouhé minimálně 6 znaků";
					break;
				case StatusCode.NotValidID:
					msg = "Nebyla vybrána správná třída či pohlaví";
					break;
				case StatusCode.AlreadyExists:
					msg = "Tento uživatelský účet již existuje";
					break;
				case StatusCode.JustALittleError:
					return RedirectToAction("Login", new {ShowRegistrationErrorElement = true});
			}
			if (!res.IsSuccess) {
				m.ClearPasswords();
				this.AddErrorToastMessage(msg);
				return View(m);
			}
			return View("RegisterSuccess");
		}

#endregion

#region Forgot Password & Change Password

#region Forgot password (enter email)

		[HttpGet]
		public ActionResult ForgotPassword()
		{
			var model = new ForgetPwdModel();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgetPwdModel model)
		{
			if (!ModelState.IsValid || !model.IsValid) {
				this.AddErrorToastMessage("Vstupní údaje nejsou platné");
				return View(model);
			}

			if (!await IsRecaptchaValidAsync()) {
				this.AddErrorToastMessage("Údajně jste robot!");
				return View(model);
			}
			var url = Url.Action("ChangePassword", "Account", new {Area = ""}, "https");
			var res = await Mgr.ForgotPasswordAsync(model.Email, url);
			if (res.IsSuccess || res.Status == StatusCode.NotFound) {
				// Neinformovat, že daný uživatel neexistuje
				this.AddSuccessToastMessage("Instrukce byly odeslány na zadaný email");
				return this.RedirectToLogin();
			}
			var msg = res.GetStatusMessage();
			switch (res.Status) {
				case StatusCode.NotEnabled:
					return RedirectToAction("Login", new {ShowActivationElement = true});
				case StatusCode.InternalError:
					msg = "Nezdařilo se vytvořit kód pro obnovení";
					break;
				case StatusCode.JustALittleError:
					msg = "Nezdařilo se odeslat email k odkazem pro obnovení hesla";
					break;
			}
			this.AddErrorToastMessage(msg);
			return View(model);
		}

#endregion

#region Change Password

		[HttpGet]
		public async Task<ActionResult> ChangePassword(string code)
		{
			if (string.IsNullOrWhiteSpace(code)) {
				this.AddErrorToastMessage("Daný kód není platný");
				return this.RedirectToLogin();
			}

			var mgr = this.GetManager<TokenManager>();
			var check = await mgr.IsValidAsync(code);
			if (!check.IsValid) {
				this.AddErrorToastMessage(check.GetErrorMessages());
				return this.RedirectToLogin();
			}

			var model = new ChangePasswordModel() {
				Code = code
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(ChangePasswordModel.Code))]
		public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
		{
			if (!ModelState.IsValid || model == null || !model.IsValid) {
				this.AddErrorToastMessage("Zadané údaje nejsou platné");
				return View(model);
			}

			var tokenMgr = this.GetManager<TokenManager>();
			var check = await tokenMgr.IsValidAsync(model.Code);
			if (!check.IsValid || check.Token == null) {
				this.AddErrorToastMessage(check.GetErrorMessages());
				return this.RedirectToLogin();
			}
			var password = model.Password;
			if (password != model.RepeatPassword) {
				this.AddErrorToastMessage("Hesla se neshodují");
				model.ClearPasswords();
				return View(model);
			}

			var validation = await Mgr.PasswordValidator.ValidateAsync(password);
			foreach (var error in validation.Errors) {
				this.AddErrorToastMessage(error);
				model.ClearPasswords();
				return View();
			}

			var token = check.Token;
			await tokenMgr.UseAsync(token);

			var res = await Mgr.ResetPasswordAsync(model, token);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage(res.GetStatusMessage());
				return this.RedirectToLogin();
			}

			this.AddSuccessToastMessage("Heslo bylo úspěšně změněno");
			return this.RedirectToLogin();
		}

#endregion

#endregion

#region Activation & Resend activation code

		[HttpGet]
		public async Task<ActionResult> Activate(string code)
		{
			if (string.IsNullOrWhiteSpace(code)) {
				this.AddErrorToastMessage("Aktivační kód není platný");
				return this.RedirectToLogin();
			}

			var tokenMgr = this.GetManager<TokenManager>();
			var check = await tokenMgr.IsValidAsync(code);
			if (!check.IsValid || check.Token == null) {
				this.AddErrorToastMessage(check.GetErrorMessages());
				return this.RedirectToLogin();
			}

			var token = check.Token;
			await tokenMgr.UseAsync(token);
			var res = await Mgr.ConfirmEmailAsync(token.UserID);
			res = await Mgr.ActivateAsync(token.UserID);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage(res.GetStatusMessage());
				return this.RedirectToLogin();
			}
			this.AddSuccessToastMessage("Účet byl úspěšně aktivován");
			return RedirectToLogin();
		}

#region Resend code

		[HttpGet]
		public ActionResult ReSend(string email)
		{
			var model = new ActivationModel();
			if (!string.IsNullOrWhiteSpace(email)) {
				model.Email = email;
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ReSend(ActivationModel model)
		{
			if (!ModelState.IsValid || model == null || !model.IsValid) {
				this.AddErrorToastMessage("Zadané údaje nejsou platné");
				return View(model);
			}
			if (!await IsRecaptchaValidAsync()) {
				this.AddErrorToastMessage("Údajně jste robot!");
				return View(model);
			}
			var url = Url.Action("Activate", "Account", new {Area = ""}, "https");
			var res = await Mgr.ResendActivationAsync(model.Email, url);
			if (res.IsSuccess || res.Status == StatusCode.NotFound || res.Status == StatusCode.NotEnabled) {
				// Neinformovat, že daný uživatel neexistuje
				this.AddSuccessToastMessage("Instrukce byly odeslány na zadaný email");
				return this.RedirectToLogin();
			}
			var msg = res.GetStatusMessage();
			switch (res.Status) {
				case StatusCode.InternalError:
					msg = "Nezdařilo se vytvořit kód pro aktivaci účtu";
					break;
				case StatusCode.JustALittleError:
					msg = "Nezdařilo se odeslat email k odkazem pro aktivaci účtu";
					break;
			}
			this.AddErrorToastMessage(msg);
			return View(model);
		}

#endregion

#endregion

#region Change email

		[HttpGet]
		public async Task<ActionResult> ChangeEmail(string code)
		{
			if (string.IsNullOrWhiteSpace(code)) {
				this.AddErrorToastMessage("Kód není platný");
				return this.RedirectToLogin();
			}
			var tokenMgr = this.GetManager<TokenManager>();
			var check = await tokenMgr.IsValidAsync(code);
			if (!check.IsValid || check.Token == null) {
				this.AddErrorToastMessage(check.GetErrorMessages());
				return this.RedirectToLogin();
			}
			var token = check.Token;
			await tokenMgr.UseAsync(token);
			var res = await Mgr.ChangeEmailConfirmAsync(token.UserID);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage(res.GetStatusMessage());
				return this.RedirectToLogin();
			}
			var sMgr = this.GetManager<SignInManager>();
			sMgr.SignOut();
			this.AddSuccessToastMessage("Email byl úspěšně změněn, nyní se přihlašte");
			return RedirectToLogin();
		}

#endregion

#region Get info

		[Authorize]
		[OutputCache(Duration = int.MaxValue, VaryByParam = "userId;c")]
		public async Task<ActionResult> ProfilePhoto(int userId)
		{
			if (User.IsInRole(UserRoles.Public)) {
				return new HttpUnauthorizedResult();
			}
			if (userId < 1) {
				return new HttpUnauthorizedResult();
			}

			var res = await Mgr.GetByIdAsync(userId);
			if (!res.IsSuccess) {
				return new HttpNotFoundResult("Could not find profile photo");
			}
			var img = res.Content.ProfilePhoto;
			if (img == null) {
				return new HttpNotFoundResult("Could not find profile photo");
			}
			var bytes = Convert.FromBase64String(img.Base64);
			return new FileContentResult(bytes, img.MIME);
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult> GetStudents(StudentsDrop model)
		{
			if (User.IsInRole(UserRoles.Public)) {
				return AccessDenied();
			}
			if (model == null || this.User.Identity.GetUserId() != model.Uqid) {
				return AccessDenied();
			}
			var schoolId = this.User.Identity.GetSchoolId();
			var loggedId = this.User.Identity.GetId();
			var students = await Mgr.GetStudents(schoolId, loggedId);
			if (students == null || students.Count < 1) {
				return AccessDenied();
			}
			model.Students = students.ToList();
			return PartialView(model);
		}

		[AllowAnonymous]
		[HttpGet]
		[OutputCache(Duration = int.MaxValue, VaryByParam = "schoolId;c")]
		public async Task<ActionResult> GetClasses(int schoolId)
		{
			var cMgr = this.GetManager<ClassManager>();
			var classes = await cMgr.GetClassJsonAsync(schoolId, true);
			var model = classes.Select(x => new SelectListItem() {
				Value = x.Item1.ToString(),
				Text = x.Item2,
				Selected = false
			});

			return PartialView(model);
		}

		[Authorize(Roles = UserRoles.AdminOrDeveloper)]
		[HttpGet]
		[OutputCache(Duration = int.MaxValue, VaryByParam = "c")]
		public async Task<ActionResult> GetSchools()
		{
			var sMgr = this.GetManager<SchoolManager>();
			var schools = await sMgr.GetAllAsync();
			var model = schools.Select(x => new SelectListItem() {
				Value = x.ID.ToString(),
				Text = x.Name,
				Selected = false
			});
			return PartialView(model);
		}

#endregion

#region WakeUp

		public ActionResult WakeUp()
		{
			return new EmptyResult();
		}

		public async Task<JsonResult> DbConnection()
		{
			using (ZoliksEntities ent = new ZoliksEntities()) {
				var conn = ent.Database.Connection;
				await conn.OpenAsync();
				conn.Close();
				conn.Dispose();
				return Json(true, JsonRequestBehavior.AllowGet);
			}
		}

#endregion

#endregion
	}
}