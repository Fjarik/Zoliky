using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SharedLibrary.Enums;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.App.Models;
using ZolikyWeb.Areas.App.Models.Account;
using ZolikyWeb.Models.Account;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;
using ChangePasswordModel = ZolikyWeb.Areas.App.Models.Account.ChangePasswordModel;

namespace ZolikyWeb.Areas.App.Controllers
{
	[Authorize]
	public class AccountController : OwnController<UserManager>
	{
		public ActionResult Index()
		{
			return RedirectToAction("Settings", "Account", new {Area = "App"});
		}

		public async Task<ActionResult> LoginHistory()
		{
			var id = this.User.Identity.GetId();
			var mgr = this.GetManager<UserLoginManager>();

			var res = await mgr.GetAllAsync(id);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage("Nezadřilo se načíst historii přihlášení");
				return RedirectToApp();
			}

			return View(res.Content);
		}

#region Settings

		[HttpGet]
		public async Task<ActionResult> Settings(int tabId = 1)
		{
			var u = await this.GetLoggedUserAsync();
			var isTester = User.Identity.IsTester();
			var setMgr = this.GetManager<UserSettingManager>();
			var settingsRes = await setMgr.GetAllByUserIdAsync(u.ID);
			var settings = settingsRes.Content;
			if (!settingsRes.IsSuccess) {
				settings = new List<UserSetting>();
			}
			var model = new SettingsModel(u, isTester, settings, tabId);
			return View(model);
		}

		private async Task<ActionResult> SaveSettings(IDictionary<string, bool> dictionary, int tabId)
		{
			if (!this.IsAuthenticated()) {
				return RedirectToLogin();
			}

			if (dictionary == null) {
				this.AddErrorToastMessage("Nezdařilo se uložit změny");
				return RedirectToAction("Settings", new {tabId});
			}

			var mgr = this.GetManager<UserSettingManager>();
			var id = this.User.Identity.GetId();

			foreach (var elm in dictionary) {
				var res = await mgr.CreateAsync(id, elm.Key, elm.Value, true);
				if (!res.IsSuccess) {
					this.AddErrorToastMessage("Nezdařilo se uložit změny");
					return RedirectToAction("Settings", new {tabId});
				}
			}

			this.AddSuccessToastMessage("Nastavení bylo uloženo");
			return RedirectToAction("Settings", new {tabId});
		}

#region General

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeProfilePhoto()
		{
			if (!this.IsAuthenticated()) {
				return RedirectToLogin();
			}

#region File Check

			if (Request.Files.Count != 1) {
				this.AddErrorToastMessage("Neplatný počet souborů");
				return RedirectToAction("Settings");
			}

			var file = Request.Files[0];
			if (file == null || file.ContentLength < 1) {
				this.AddErrorToastMessage("Neplatný soubor");
				return RedirectToAction("Settings");
			}

			if (file.ContentLength > 4096 * 1024) {
				this.AddErrorToastMessage("Soubor může mít maximálně 4MB");
				return RedirectToAction("Settings");
			}

			if (!file.IsImage()) {
				this.AddErrorToastMessage("Musíte vybrat obrázek(.jpg, .png, .gif)");
				return RedirectToAction("Settings");
			}

#endregion

			var mime = file.ContentType;

			byte[] bytes;
			using (BinaryReader br = new BinaryReader(file.InputStream)) {
				bytes = br.ReadBytes((int) file.InputStream.Length);
			}

			var userId = this.User.Identity.GetId();

			var imgMgr = this.GetManager<ImageManager>();
			var res = await imgMgr.CreateAsync(userId, bytes, mime);
			if (res.IsSuccess) {
				var img = res.Content;
				await Mgr.ChangeProfilePhotoAsync(userId, img.ID);
				Session.Add(Globals.CacheBurst, (int) DateTime.Now.Ticks);
			} else {
				this.AddErrorToastMessage("Nezařilo se změnit profilový obrázek");
			}
			return RedirectToAction("Settings");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeGeneral(GeneralChangeModel model)
		{
			if (model == null || !ModelState.IsValid || !model.IsValid) {
				this.AddErrorToastMessage("Zadané údaje nejsou platné");
				return RedirectToAction("Settings");
			}

			var logged = await this.GetLoggedUserAsync();
			if (logged.Sex != model.Gender) {
				logged.Sex = model.Gender;
				await Mgr.SaveAsync(logged);
			}

			var newEmail = model.Email.Trim().ToLower();

			if (logged.Email != newEmail) {
				return await ChangeEmailAsync(logged.ID, model.Email);
			}

			this.AddSuccessToastMessage("Změny úspěšně uloženy");
			return RedirectToAction("Settings");
		}

		private async Task<ActionResult> ChangeEmailAsync(int userId, string newEmail)
		{
			var url = Url.Action("ChangeEmail", "Account", new {Area = ""}, "https");
			var res = await Mgr.ChangeEmailAsync(userId, newEmail, url);
			if (res.IsSuccess) {
				this.AddSuccessToastMessage("Potvrzovací email byl odeslán na zadaný email");
				return this.RedirectToApp();
			}
			var msg = res.GetStatusMessage();
			switch (res.Status) {
				case StatusCode.InternalError:
					msg = "Nezdařilo se vytvořit kód pro změnu emailu";
					break;
				case StatusCode.JustALittleError:
					msg = "Nezdařilo se odeslat email k odkazem pro změnu emailu";
					break;
			}
			this.AddErrorToastMessage(msg);
			return RedirectToAction("Settings");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateSecureHiddenInputs(nameof(Models.Account.ChangePasswordModel.CurrentUserGuid))]
		public async Task<ActionResult> ChangePassword(Models.Account.ChangePasswordModel model)
		{
			if (model == null || !ModelState.IsValid || !model.IsValid) {
				this.AddErrorToastMessage("Zadané údaje nejsou platné");
				return RedirectToAction("Settings");
			}

			var validation = await Mgr.PasswordValidator.ValidateAsync(model.Password);
			foreach (var error in validation.Errors) {
				this.AddErrorToastMessage(error);
				return RedirectToAction("Settings");
			}

			var logged = await this.GetLoggedUserAsync();
			if (logged == null) {
				this.AddErrorToastMessage("Nejste přihlášený");
				return RedirectToLogin();
			}

			if (!Guid.TryParse(model.CurrentUserGuid, out Guid guid) || logged.UQID != guid) {
				this.AddErrorToastMessage("Neplatný uživatel");
				return RedirectToAction("Settings");
			}

			var res = await Mgr.ChangePasswordAsync(logged, model.OldPassword, model.Password);
			if (!res.IsSuccess) {
				this.AddErrorToastMessage(res.GetStatusMessage());
				return RedirectToAction("Settings");
			}

			var signInMgr = this.GetManager<SignInManager>();
			signInMgr.SignOut();

			this.AddSuccessToastMessage("Heslo bylo úspěšně změněno");
			this.AddWarningToastMessage("Nyní se musíte znovu přihlásit s novým heslem");
			return RedirectToLogin();
		}

#endregion

#region Privacy

		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<ActionResult> EditPrivacy(PrivacyModel model)
		{
			return SaveSettings(model?.Dictionary, 2);
		}

		/*
		[HttpGet]
		public ActionResult Remove()
		{
			// TODO: Odstranění účtů
			return View();
		}
		*/

#endregion

#region Notifications

		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<ActionResult> EditNotifications(NotificationModel model)
		{
			return SaveSettings(model?.Dictionary, 3);
		}

#endregion

#region External Login

#region Add

		[HttpPost]
		[ValidateSecureHiddenInputs(nameof(ExternalLoginModel.Provider))]
		public ActionResult AddLogin(ExternalLoginModel loginModel)
		{
			if (loginModel == null) {
				return RedirectToAction("Settings", "Account", new {Area = "App", tabId = 4});
			}

			var id = User.Identity.GetUserId();
			return new ChallengeResult(loginModel.Provider,
									   Url.Action("AddLoginCallback", "Account", new {Area = "App"}),
									   id);
		}

		[HttpGet]
		public async Task<ActionResult> AddLoginCallback()
		{
			var id = User.Identity.GetUserId();
			var loginInfo = await HttpContext.GetOwinContext()
											 .Authentication
											 .GetExternalLoginInfoAsync(ChallengeResult.XsrfKey, id);
			if (loginInfo == null) {
				this.AddErrorToastMessage("Nezdařilo se načíst data od externího poskytovatele");
				return RedirectToAction("Settings", "Account", new {Area = "App", tabId = 4});
			}

			var user = await this.GetLoggedUserAsync();
			var mgr = this.GetManager<LoginTokenManager>();

			string provider = loginInfo.Login.LoginProvider;
			string key = loginInfo.Login.ProviderKey;

			var res = await mgr.CreateAsync(user.ID, provider, key);

			if (res.Status == StatusCode.AlreadyExists) {
				this.AddErrorToastMessage($"Tento {provider} účet je již napojen k jinému účtu na Žolíkách");
			} else {
				if (!res.IsSuccess) {
					string msg = res.GetStatusMessage();
					this.AddErrorToastMessage($"Nezdařilo se vytvořit propojení s externí síťí. Chyba: {msg}");
				} else {
					this.AddSuccessToastMessage($"{provider} účet byl úspěšně napojen");
				}
			}

			return RedirectToAction("Settings", "Account", new {Area = "App", tabId = 4});
		}

#endregion

#region Remove

		[HttpPost]
		[ValidateSecureHiddenInputs(nameof(ExternalLoginModel.Provider))]
		public async Task<ActionResult> RemoveLogin(ExternalLoginModel loginModel)
		{
			if (loginModel == null) {
				return RedirectToAction("Settings", "Account", new {Area = "App", tabId = 4});
			}

			var user = await this.GetLoggedUserAsync();
			string provider = loginModel.Provider;
			var mgr = this.GetManager<LoginTokenManager>();
			var res = await mgr.DeleteAsync(user.ID, provider);
			if (res) {
				this.AddSuccessToastMessage($"{provider} účet byl úspěšně odpojen");
			} else {
				this.AddErrorToastMessage($"Nezdařilo se odpojit {provider} účet");
			}

			return RedirectToAction("Settings", "Account", new {Area = "App", tabId = 4});
		}

#endregion

#endregion

#region Tester

		[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<ActionResult> SwitchTester()
		{
			var mgr = this.GetManager<SignInManager>();
			var res = await mgr.SwitchTesterAsync();
			if (!res) {
				this.AddErrorToastMessage("Nezdařilo se zapnout testerské funkce");
				return RedirectToAction("Settings");
			}

			this.AddSuccessToastMessage("Testerské funkce " + (!this.User.Identity.IsTester()
																   ? "zapnuty"
																   : "vypnuty"));
			return RedirectToApp();
		}

#endregion

#endregion
	}
}