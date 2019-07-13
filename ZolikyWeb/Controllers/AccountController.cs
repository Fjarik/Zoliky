using System;
using System.Collections.Generic;
using System.Linq;
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
				Project = Projects.Web,
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
				case StatusCode.EmailNotConfirmed:
					lg.ShowActivationElement = true;
					break;
				case StatusCode.OK:
					if (res.Content is User u) {
						this.SignInManager.SignIn(u, lg.RememberMe);
						this.AddSuccessToastMessage("Přihlášení proběhlo úspěšně");
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
			var res = await Mgr.LoginAsync(loginInfo, Request.GetIPAddress(), Projects.Web);
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
					msg = $"Tento {loginInfo.Login.LoginProvider} účet není připojen k žádnému účtu žolíků";
					break;
				case StatusCode.WrongPassword:
					msg = "Nesprávné jméno nebo heslo";
					break;
				case StatusCode.ExpiredPassword:
					msg = "Vašemu heslu vypršela platnost";
					break;
				case StatusCode.NotEnabled:
				case StatusCode.EmailNotConfirmed:
					lg.ShowActivationElement = true;
					break;
				case StatusCode.OK:
					if (res.Content is User u) {
						SignInManager.SignIn(u, false);
						this.AddSuccessToastMessage("Přihlášení proběhlo úspěšně");
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

#endregion

#region Logout

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[OfflineActionFilter(Active = false)]
		public ActionResult Logout(string r)
		{
			SignInManager.SignOut();
			this.AddSuccessToastMessage("Ohlášení proběhlo úspěšně");
			if (Url.IsLocalUrl(r)) {
				return Redirect(r);
			}
			return RedirectToAction("Login");
		}

#endregion

#endregion

#region Registration

		[HttpGet]
		public async Task<ActionResult> Register()
		{
			var cMgr = this.GetManager<ClassManager>();
			var classes = await cMgr.GetAllAsync();
			var model = new RegisterModel {
				Classes = classes
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterModel m)
		{
			if (m == null) {
				m = new RegisterModel();
			}
			var cMgr = this.GetManager<ClassManager>();
			m.Classes = await cMgr.GetAllAsync();
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
			var res = await Mgr.RegisterAsync(m.Email,
											  m.Password,
											  m.Firstname,
											  m.Lastname,
											  m.Username,
											  (byte) m.Gender,
											  classId,
											  1,
											  m.Newsletter,
											  m.FutureNews,
											  this.Request.GetIPAddress(),
											  url);
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

#region Get user info

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
		public ActionResult GetStudents(StudentsDrop model)
		{
			if (User.IsInRole(UserRoles.Public)) {
				return AccessDenied();
			}
			if (model == null || this.User.Identity.GetUserId() != model.Uqid) {
				return AccessDenied();
			}
			var loggedId = this.User.Identity.GetId();
			var students = Mgr.GetStudents(loggedId);
			if (students == null || students.Count < 1) {
				return AccessDenied();
			}
			model.Students = students.ToList();
			return PartialView(model);
		}

#endregion

#endregion
	}
}