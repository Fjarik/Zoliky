﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared.ApiModels;
using ImpromptuInterface;
using Dynamitey;
using Newtonsoft.Json;
using SharedLibrary.Shared;

namespace DataAccess.Managers.New
{
	public sealed class UserManager : Manager<User>
	{
		/// 
		/// Fields
		///
		public PasswordValidator PasswordValidator { get; private set; }

		/// 
		/// Constructors
		/// 
		private UserManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		private UserManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

		public override async Task<MActionResult<User>> GetByIdAsync(int id)
		{
			if (id < 1) {
				return new MActionResult<User>(StatusCode.NotValidID);
			}

			var res = await base.GetByIdAsync(id);
			if (!res.IsSuccess) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			User u = res.Content;
			if (!u.Enabled) {
				return new MActionResult<User>(StatusCode.NotEnabled, u);
			}

			//u.Password = "";
			return new MActionResult<User>(StatusCode.OK, u);
		}

		public override async Task<bool> DeleteAsync(int userId)
		{
			if (userId < 1) {
				return false;
			}
			var res = await this.GetByIdAsync(userId);
			if (!res.IsSuccess && res.Status != StatusCode.NotEnabled) {
				return false;
			}
			return await this.DeleteAsync(res.Content);
		}

		public override async Task<bool> DeleteAsync(User u)
		{
			if (u == null) {
				return false;
			}
			try {
				if (u.IsInRolesOr(UserRoles.Administrator,
								  UserRoles.Developer,
								  UserRoles.Teacher,
								  UserRoles.LoginOnly,
								  UserRoles.Support)) {
					return false;
				}
				var id = u.ID;

#region Passwords

				u.PasswordID = null;
				await this.SaveAsync(u);


				var passwords = _ctx.SomeHashes.Where(x => x.OwnerID == id);
				_ctx.SomeHashes.RemoveRange(passwords);

#endregion

#region Relations

				var consents = _ctx.Consents.Where(x => x.UserID == id);
				_ctx.Consents.RemoveRange(consents);

				var crashes = _ctx.Crashes.Where(x => x.UserID == id);
				_ctx.Crashes.RemoveRange(crashes);
				await _ctx.SaveChangesAsync();

				var images = _ctx.Images.Where(x => x.OwnerID == id);
				_ctx.Images.RemoveRange(images);
				await _ctx.SaveChangesAsync();

				var notifications = _ctx.Notifications.Where(x => x.FromID == id || x.ToID == id);
				_ctx.Notifications.RemoveRange(notifications);
				await _ctx.SaveChangesAsync();

				var tokens = _ctx.Tokens.Where(x => x.UserID == id);
				_ctx.Tokens.RemoveRange(tokens);

				var logins = _ctx.UserLogins.Where(x => x.UserID == id);
				_ctx.UserLogins.RemoveRange(logins);
				await _ctx.SaveChangesAsync();

				var loginTokens = _ctx.UserLoginTokens.Where(x => x.UserID == id);
				_ctx.UserLoginTokens.RemoveRange(loginTokens);
				await _ctx.SaveChangesAsync();

				var settings = _ctx.UserSettings.Where(x => x.ID == id);
				_ctx.UserSettings.RemoveRange(settings);
				await _ctx.SaveChangesAsync();

				var events = _ctx.WebEvents.Where(x => x.FromID == id || x.ToID == id);
				_ctx.WebEvents.RemoveRange(events);
				await _ctx.SaveChangesAsync();

#endregion

#region Zoliks

				var originals = _ctx.Zoliky.Where(x => x.OriginalOwnerID == id);
				foreach (var zolik in originals) {
					_ctx.Zoliky.Attach(zolik);
					zolik.OriginalOwnerID = Ext.BankId;
				}
				await _ctx.SaveChangesAsync();

				var zolikMgr = this.Context.Get<ZolikManager>();
				foreach (var zolik in _ctx.Zoliky.Where(x => x.OwnerID == id)) {
					var del = await zolikMgr.DeleteAsync(zolik);
					if (!del) {
						return false;
					}
				}

#endregion

#region Transactions

				foreach (var tran in _ctx.Transactions.Where(x => x.ToID == id || x.FromID == id)) {
					_ctx.Transactions.Attach(tran);
					if (tran.FromID == id) {
						tran.FromID = Ext.BankId;
					}
					if (tran.ToID == id) {
						tran.ToID = Ext.BankId;
					}
				}
				await _ctx.SaveChangesAsync();

#endregion

#region Many to Many

				_ctx.Users.Attach(u);
				u.UserSettings.Clear();
				u.Roles.Clear();
				u.ReadNotifications.Clear();
				u.Achievements.Clear();
				u.LoginTokens.Clear();
				await SaveAsync(u);

#endregion

				await _ctx.Entry(u).ReloadAsync();
				_ctx.Users.Remove(u);
				await _ctx.SaveChangesAsync();

				return true;
			} catch {
#if DEBUG
				throw;
#endif
				return false;
			}
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
		{
			var mgr = new UserManager(context) {
				PasswordValidator = new PasswordValidator {
					RequireDigit = false,
					RequireLowercase = false,
					RequireNonLetterOrDigit = false,
					RequireUppercase = false,
					RequiredLength = 5,
				}
			};

			return mgr;
		}

#endregion

#region Own Methods

#region Select

		public async Task<MActionResult<User>> SelectAsync(Guid input)
		{
			return await this.SelectAsync(input.ToString(), WhatToCheck.Guid);
		}

		public async Task<MActionResult<User>> SelectAsync(string input)
		{
			if (string.IsNullOrWhiteSpace(input)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			if (input.Contains('@')) {
				return await SelectAsync(input, WhatToCheck.Email);
			}

			if (input.Split('-').Length == 4) {
				return await SelectAsync(input, WhatToCheck.Guid);
			}
			return await SelectAsync(input, WhatToCheck.Username);
		}

		public async Task<MActionResult<User>> SelectAsync(string input, WhatToCheck check)
		{
			if (string.IsNullOrWhiteSpace(input)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			input = input.Trim().ToLower();

			if (input.Contains("petatrunda")) {
				input = input.Replace("petatrunda", "zerzpe");
			}

			if (input.Contains('@')) {
				check = WhatToCheck.Email;
			}

			User u = null;
			switch (check) {
				case WhatToCheck.Username:
					u = await _ctx.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username.Trim().ToLower() == input);
					break;
				case WhatToCheck.Email:
					u = await _ctx.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Trim().ToLower() == input);
					break;
				case WhatToCheck.Guid:
					if (Guid.TryParse(input, out Guid uqid) && uqid != Guid.Empty) {
						u = await _ctx.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UQID == uqid);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(check), check, null);
			}

			if (u == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			if (!u.Enabled) {
				return new MActionResult<User>(StatusCode.NotEnabled, u);
			}
			return new MActionResult<User>(StatusCode.OK, u);
		}

		public async Task<int> GetIdByGuidAsync(Guid guid)
		{
			if (guid == Guid.Empty) {
				return Ext.IgnoreId;
			}
			var res = await _ctx.Users.Where(x => x.UQID == guid).Select(x => x.ID).FirstOrDefaultAsync();
			if (res < 1) {
				return Ext.IgnoreId;
			}
			return res;
		}

#endregion

#region Login

		public async Task<MActionResult<User>> LoginAsync(ExternalLoginInfo info,
														  string ip,
														  Projects project,
														  bool recordLogin = true)
		{
			if (info?.Login == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			return await this.LoginAsync(info.Login, ip, project, recordLogin);
		}

		private async Task<MActionResult<User>> LoginAsync(UserLoginInfo info,
														   string ip,
														   Projects project,
														   bool recordLogin = true)
		{
			if (info == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			var mgr = Context.Get<LoginTokenManager>();
			var res = await mgr.GetAsync(info);
			if (!res.IsSuccess) {
				return new MActionResult<User>(res.Status);
			}
			var token = res.Content;

			var uRes = await this.GetByIdAsync(token.UserID);
			return await this.CheckLoginAsync(uRes, ip, project, recordLogin);
		}

		public async Task<MActionResult<User>> LoginAsync(Logins lg, string ip, bool recordLogin = true)
		{
			if (lg == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			if (lg.Project == 0 || lg.Project == Projects.Unknown) {
				return await this.LoginAsync(lg.UName, lg.Password, ip);
			}
			return await this.LoginAsync(lg.UName, lg.Password, ip, lg.Project, recordLogin);
		}

		public Task<MActionResult<User>> LoginAsync(string username,
													string password,
													string ip)
		{
			return LoginAsync(username, password, ip, Projects.Unknown, false);
		}

		private async Task<MActionResult<User>> LoginAsync(string username,
														   string password,
														   string ip,
														   Projects project,
														   bool recordLogin = true)
		{
			if (string.IsNullOrWhiteSpace(username)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			var userRes = await this.SelectAsync(username);
			if (!userRes.IsSuccess && userRes.Status != StatusCode.NotEnabled) {
				return userRes;
			}

			return await LoginAsync(userRes.Content, password, ip, project, recordLogin);
		}

		private async Task<MActionResult<User>> LoginAsync(User u,
														   string password,
														   string ip,
														   Projects project,
														   bool recordLogin = true)
		{
			if (string.IsNullOrWhiteSpace(password)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			if (u == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}
			var pwdHash = u.Password;
			if (pwdHash == null) {
				return new MActionResult<User>(StatusCode.NoPassword);
			}
			if (pwdHash.IsExpired) {
				return new MActionResult<User>(StatusCode.ExpiredPassword);
			}
			var loginMgr = Context.Get<UserLoginManager>();
			var hashMgr = Context.Get<HashManager>();

			var res = hashMgr.VerifyHashedPassword(u.ID, pwdHash, password);
			switch (res) {
				case PasswordVerificationResult.Failed:
					if (recordLogin) {
						await loginMgr.CreateAsync(u.ID, project, LoginStatus.BadPass, ip);
					}
					return new MActionResult<User>(StatusCode.WrongPassword);
				case PasswordVerificationResult.SuccessRehashNeeded:
					var pwdRes = await hashMgr.CreatePasswordAsync(password, u.ID);
					if (!pwdRes.IsSuccess) {
						return new MActionResult<User>(pwdRes.Status);
					}
					var hash = pwdRes.Content;
					var change = await ChangePasswordAsync(u, hash.ID);
					if (!change.IsSuccess) {
						return change;
					}
					u = change.Content;
					await this.AddXpAsync(10, u.ID);
					break;
				case PasswordVerificationResult.Success:
					break;
				default:
					if (recordLogin) {
						await loginMgr.CreateAsync(u.ID, project, LoginStatus.Error, ip);
					}
					return new MActionResult<User>(StatusCode.InternalError);
			}
			return await this.CheckLoginAsync(loginMgr, u, ip, project, recordLogin);
		}

		private async Task<MActionResult<User>> CheckLoginAsync(MActionResult<User> uRes,
																string ip,
																Projects project,
																bool recordLogin = true)
		{
			if (uRes == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			if (!uRes.IsSuccess && uRes.Status != StatusCode.NotEnabled) {
				return uRes;
			}
			var loginMgr = this.Context.Get<UserLoginManager>();
			return await CheckLoginAsync(loginMgr, uRes.Content, ip, project, recordLogin);
		}

		private async Task<MActionResult<User>> CheckLoginAsync(UserLoginManager loginMgr,
																User u,
																string ip,
																Projects project,
																bool recordLogin = true)
		{
			if (!u.Enabled) {
				return new MActionResult<User>(StatusCode.NotEnabled, u);
			}

			if (u.IsInRole(UserRoles.Robot)) {
				return new MActionResult<User>(StatusCode.InsufficientPermissions);
			}
			if (recordLogin) {
				await loginMgr.CreateAsync(u.ID, project, LoginStatus.Success, ip);
			}
			return new MActionResult<User>(StatusCode.OK, u);
		}

#endregion

#region Register

		public async Task<MActionResult<User>> RegisterAsync(string email, string password,
															 string name, string lastname,
															 string username, byte gender,
															 int? classId, bool newsletter,
															 bool futureNews, string ip,
															 string url, params string[] roles)
		{
			if (!Enum.IsDefined(typeof(Sex), gender)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			return await this.RegisterAsync(email, password,
											name, lastname,
											username, (Sex) gender,
											classId, newsletter,
											futureNews, ip,
											url, roles);
		}

		public async Task<MActionResult<User>> RegisterAsync(string email, string password,
															 string name, string lastname,
															 string username, Sex gender,
															 int? classId, bool newsletter,
															 bool futureNews, string ip,
															 string url, params string[] roles)
		{
			var r = roles.Distinct().ToList();
			if (classId == null) {
				r.RemoveAll(x => string.Equals(x, UserRoles.Student, StringComparison.CurrentCultureIgnoreCase));
				r.Add(UserRoles.Public);
			} else {
				if (!r.Contains(UserRoles.Student)) {
					r.Add(UserRoles.Student);
				}
			}
			var rolesFinal = await _ctx.Roles.Where(x => r.Contains(x.Name)).ToArrayAsync();

			return await this.RegisterAsync(email, password,
											name, lastname,
											username, gender,
											classId, newsletter,
											futureNews, ip,
											url, rolesFinal);
		}

		private async Task<MActionResult<User>> RegisterAsync(string email, string password,
															  string name, string lastname,
															  string username, Sex gender,
															  int? classId, bool newsletter,
															  bool futureNews, string ip,
															  string url, params Role[] roles)
		{
			if (Methods.AreNullOrWhiteSpace(email, password, name, lastname, username, url) ||
				!Methods.IsEmailValid(email) ||
				!roles.Any()) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			var validResult = await PasswordValidator.ValidateAsync(password);
			if (!validResult.Succeeded) {
				return new MActionResult<User>(StatusCode.WrongPassword);
			}
			if (classId != null && classId < 1) {
				return new MActionResult<User>(StatusCode.NotValidID);
			}
			if ((await this.ExistsAsync(email, WhatToCheck.Email)) ||
				(await this.ExistsAsync(username, WhatToCheck.Username))) {
				return new MActionResult<User>(StatusCode.AlreadyExists);
			}
			email = email.Trim().ToLower();
			name = name.Trim().NormalizeNames();
			lastname = lastname.Trim().NormalizeNames();
			username = username.Trim().ToLower();
			if (string.IsNullOrWhiteSpace(ip)) {
				ip = "127.0.0.1";
			}
			ip = ip.Trim();

			var u = new User() {
				ClassID = classId,
				PasswordID = null,
				UQID = Guid.NewGuid(),
				Username = username,
				Email = email,
				Name = name,
				Lastname = lastname,
				Type = UserPermission.User,
				Enabled = false,
				MemberSince = DateTime.Now,
				RegistrationIp = ip,
				Description = null,
				XP = 0,
				ProfilePhotoID = Ext.DefaultProfilePhotoId,
				Sex = gender,
				VersionS = "1.0.1",
			};
			foreach (var role in roles) {
				var attached = _ctx.Roles.Attach(role);
				u.Roles.Add(attached);
			}

			var registrationRes = await this.CreateAsync(u);
			if (!registrationRes.IsSuccess) {
				return registrationRes;
			}
			u = registrationRes.Content;

			var hashMgr = this.Context.Get<HashManager>();
			var pwdRes = await hashMgr.CreatePasswordAsync(password, u.ID);
			if (!pwdRes.IsSuccess) {
				return new MActionResult<User>(pwdRes.Status);
			}
			var hash = pwdRes.Content;

			u.PasswordID = hash.ID;
			await this.SaveAsync(u);

			var settingsMgr = this.Context.Get<UserSettingManager>();
			var setSettings = new List<string> {SettingKeys.LeaderboardZolik,
												SettingKeys.VisibleRank,
												SettingKeys.VisibleZolik
			};
			if (newsletter) {
				setSettings.Add(SettingKeys.Newsletter);
			}
			if (futureNews) {
				setSettings.Add(SettingKeys.FutureNews);
			}
			foreach (var key in setSettings) {
				var res = await settingsMgr.CreateAsync(u.ID, key, true);
			}

			var tokenMgr = this.Context.Get<TokenManager>();
			var tokenRes = await tokenMgr.CreateActivationTokenAsync(u.ID);
			if (!tokenRes.IsSuccess) {
				return new MActionResult<User>(StatusCode.JustALittleError);
			}
			var token = tokenRes.Content;
			var code = tokenMgr.GenerateCode(token);
			url += $"?code={code}";

			var mailMgr = this.Context.Get<MailManager>();
			var mail = await mailMgr.RegisterAsync(u, url);
			if (!mail) {
				return new MActionResult<User>(StatusCode.JustALittleError);
			}
			var adminMail = await mailMgr.AdminRegAsync(u, u.RegistrationIp);

			return new MActionResult<User>(StatusCode.OK, u);
		}

#endregion

#region Exists & Check

		public async Task<bool> ExistsAsync(string input, WhatToCheck what)
		{
			if (string.IsNullOrWhiteSpace(input)) {
				return false;
			}

			input = input.Trim().ToLower();

			bool exists;

			switch (what) {
				case WhatToCheck.Username:
					exists = await _ctx.Users.AnyAsync(x => x.Username.Trim().ToLower() == input);
					break;
				case WhatToCheck.Email:
					exists = await _ctx.Users.AnyAsync(x => x.Email.Trim().ToLower() == input);
					break;
				case WhatToCheck.Guid:
					if (!Guid.TryParse(input, out Guid uqid) || uqid == Guid.Empty) {
						return false;
					}
					exists = await ExistsAsync(uqid);
					break;
				default:
					return false;
			}

			return exists;
		}

		public async Task<bool> ExistsAsync(Guid g)
		{
			if (await _ctx.Users.AnyAsync(x => x.UQID == g)) {
				return true;
			}
			return false;
		}

		public async Task<bool> CheckIdAndUqidAsync(int id, string uqid)
		{
			if (id < 1 || string.IsNullOrWhiteSpace(uqid) || !Guid.TryParse(uqid, out Guid g)) {
				return false;
			}
			return await _ctx.Users.AnyAsync(x => x.ID == id && x.UQID == g);
		}

#endregion

#region Student

		public async Task<string> GetUserFullnameAsync(int id)
		{
			if (id < 1) {
				return "";
			}
			var user = await _ctx.Users.FindAsync(id);
			if (user == null) {
				return "";
			}
			return user.FullName;
		}

		public async Task<MActionResult<List<User>>> GetStudentUsersAsync(
			int? classId = null,
			bool includeImages = false,
			bool onlyActive = true)
		{
			var query = _ctx.Users
							.AsNoTracking()
							.Include(x => x.Class)
							.Include(x => x.ProfilePhoto)
							.Where(x => x.ClassID != null);
			if (classId != null) {
				query = query.Where(x => x.ClassID == classId);
			}
			var res = (await query.SortStudents()
								  .ToListAsync())
					  .Select(x => new User() {
						  ID = x.ID,
						  ClassID = x.ClassID,
						  ProfilePhotoID = x.ProfilePhotoID,
						  Name = x.Name,
						  Lastname = x.Lastname,
						  Enabled = x.Enabled,
						  Class = x.Class,
						  SpecialUsername = x.SpecialUsername,
						  ProfilePhoto = includeImages ? x.ProfilePhoto : null,
					  }).ToList();

			List<User> users = res;
			if (onlyActive) {
				users.RemoveAll(x => !x.IsEnabled);
			}
			return new MActionResult<List<User>>(StatusCode.OK, users);
		}

		public IList<IStudent> GetStudents(params int[] excludeIds)
		{
			return this.GetStudents(null,  imageMaxSize: 1, true, excludeIds).ToList<IStudent>();
		}

		public IList<GetTopStudents_Result> GetStudents(int? classId = null,
														int? imageMaxSize = null,
														bool onlyActive = true,
														params int[] excludeIds)
		{
			return _ctx.GetStudents(onlyActive, imageMaxSize, classId, excludeIds);
		}


		public IList<IStudent> GetFakeStudents(params int[] excludeIds)
		{
			return this.GetFakeStudents(1,  true, excludeIds).ToList<IStudent>();
		}

		public IList<GetTopStudents_Result> GetFakeStudents(int? imageMaxSize = null, bool onlyActive = true, params int[] excludeIds)
		{
			return _ctx.GetFakeStudents(onlyActive, imageMaxSize, excludeIds);
		}

#endregion

#region Social methods (Most zoliks,...)

		public IList<GetTopStudents_Result> GetStudentsWithMostZoliks(int top = 5, int? classId = null, int? imageMaxSize = null)
		{
			var res = _ctx.GetTopStudents(top: top, classId: classId, imageMaxSize: imageMaxSize);
			return res;
		}

		public IList<GetTopStudents_Result> GetStudentsWithMostXp(int top = 5, int? classId = null, int? imageMaxSize = null)
		{
			var res = _ctx.GetTopStudentsXp(top: top, classId: classId, imageMaxSize: imageMaxSize);
			return res;
		}

		/*
		[Obsolete]
		public async Task<MActionResult<List<User>>> GetUsersWithMostZoliksAsync(int top = 5, int? classId = null)
		{
			var query = _ctx.Users
							.AsNoTracking()
							.Where(Extensions.HasTrueSetting(SettingKeys.LeaderboardZolik))
							.Include(x => x.OriginalZoliks.Count);

			if (classId != null) {
				query = query.Where(x => x.ClassID == classId);
			}
			var res = await query.OrderByDescending(x => x.OriginalZoliks.AsQueryable().Count(Extensions.NonTesterZoliks()))
								 .Take(top)
								 .ToListAsync();
			return new MActionResult<List<User>>(StatusCode.OK, res);
		}
		
		[Obsolete]
		public Task<List<Student<Image>>> GetStudentsWithMostZoliksAsync(int top = 5, int? classId = null)
		{
			var query = _ctx.Users
							.AsNoTracking()
							.Where(x => x.ClassID != null)
							.Where(Extensions.HasTrueSetting(SettingKeys.LeaderboardZolik))
							.Include(x => x.OriginalZoliks);

			if (classId != null) {
				query = query.Where(x => x.ClassID == classId);
			}
			return query.OrderByDescending(x => x.OriginalZoliks.AsQueryable().Count(Extensions.NonTesterZoliks()))
						.SelectStudentAsync(top: top);
		}

		[Obsolete]
		public Task<List<Student<Image>>> GetStudentsWithMostXpAsync(int top = 5, int? classId = null)
		{
			var query = _ctx.Users
							.AsNoTracking()
							.Where(x => x.ClassID != null)
							.Where(Extensions.HasTrueSetting(SettingKeys.LeaderboardXp))
							.Include(x => x.OriginalZoliks);

			if (classId != null) {
				query = query.Where(x => x.ClassID == classId);
			}
			return query.OrderByDescending(x => x.XP)
						.Take(top)
						.SelectStudentAsync();
		}
		*/

#endregion

#region Change profile photo

		public async Task<MActionResult<User>> ChangeProfilePhotoAsync(int userId, int profilePhotoId)
		{
			if (userId < 1) {
				return new MActionResult<User>(StatusCode.NotValidID);
			}
			var res = await this.GetByIdAsync(userId);
			if (!res.IsSuccess) {
				return res;
			}
			return await ChangeProfilePhotoAsync(res.Content, profilePhotoId);
		}

		private async Task<MActionResult<User>> ChangeProfilePhotoAsync(User user, int profilePhotoId)
		{
			if (user == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			user.ProfilePhotoID = profilePhotoId;
			await SaveAsync(user);
			return new MActionResult<User>(StatusCode.OK, user);
		}

#endregion

#region Change, Reset & Forgot password

		#region Reset

		public async Task<MActionResult<User>> ResetPasswordAsync(IChangePasswordCode changePwd, Token token)
		{
			if (changePwd == null || !changePwd.IsValid || token == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			return await this.ResetPasswordAsync(token.UserID, changePwd.Password);
		}

		private async Task<MActionResult<User>> ResetPasswordAsync(int userId, string newPassword)
		{
			if (userId < 1) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			var res = await this.GetByIdAsync(userId);
			if (!res.IsSuccess) {
				return res;
			}
			var user = res.Content;
			return await this.ChangePasswordAsync(user, newPassword);
		}

#endregion

#region Change

		public async Task<MActionResult<User>> ChangePasswordAsync(User logged, string oldPwd, string newPwd)
		{
			if (logged == null || string.IsNullOrWhiteSpace(oldPwd) || string.IsNullOrWhiteSpace(newPwd)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			if (oldPwd == newPwd) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			var hashMgr = this.Context.Get<HashManager>();
			var oldHash = logged.Password;
			if (oldHash != null) {
				var res = hashMgr.VerifyHashedPassword(logged.ID, oldHash, oldPwd);
				if (res != PasswordVerificationResult.Success &&
					res != PasswordVerificationResult.SuccessRehashNeeded) {
					return new MActionResult<User>(StatusCode.WrongPassword);
				}
			}
			return await ChangePasswordAsync(logged, newPwd);
		}

		private async Task<MActionResult<User>> ChangePasswordAsync(User user, string newPassword)
		{
			if (string.IsNullOrWhiteSpace(newPassword)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			var validation = await this.PasswordValidator.ValidateAsync(newPassword);
			if (!validation.Succeeded) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			var hashMgr = Context.Get<HashManager>();
			var hashRes = await hashMgr.CreatePasswordAsync(newPassword, user.ID);
			if (!hashRes.IsSuccess) {
				return new MActionResult<User>(hashRes.Status);
			}
			var hash = hashRes.Content;
			return await this.ChangePasswordAsync(user, hash.ID);
		}

		private async Task<MActionResult<User>> ChangePasswordAsync(User user, int newPwdId)
		{
			if (newPwdId < 1) {
				return new MActionResult<User>(StatusCode.NotValidID);
			}
			user.PasswordID = newPwdId;
			await this.SaveAsync(user);
			return new MActionResult<User>(StatusCode.OK, user);
		}

#endregion

		public async Task<MActionResult<User>> ForgotPasswordAsync(string email, string url)
		{
			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(url)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			var res = await this.SelectAsync(email, WhatToCheck.Email);
			if (!res.IsSuccess) {
				return res;
			}
			var user = res.Content;
			if (user == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			var tokenMgr = Context.Get<TokenManager>();
			var tRes = await tokenMgr.CreateForgotPwdAsync(user.ID);
			if (!tRes.IsSuccess) {
				return new MActionResult<User>(tRes.Status);
			}
			var token = tRes.Content;
			if (token == null) {
				return new MActionResult<User>(StatusCode.InternalError);
			}

			var code = tokenMgr.GenerateCode(token);

			url += $"?code={code}";

			var mailMgr = Context.Get<MailManager>();
			var mRes = await mailMgr.ForgetPwdAsync(user, url);
			if (!mRes) {
				return new MActionResult<User>(StatusCode.JustALittleError);
			}

			return new MActionResult<User>(StatusCode.OK, user);
		}

#endregion

#region Resend & activate account

		public async Task<MActionResult<User>> ResendActivationAsync(string email, string url)
		{
			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(url)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			var res = await this.SelectAsync(email, WhatToCheck.Email);
			if (!res.IsSuccess && res.Status != StatusCode.NotEnabled) {
				return res;
			}
			var user = res.Content;
			if (user == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}
			return await this.ResendActivationAsync(user, url);
		}

		private async Task<MActionResult<User>> ResendActivationAsync(User user, string url)
		{
			var tokenMgr = Context.Get<TokenManager>();
			var tRes = await tokenMgr.CreateActivationTokenAsync(user.ID);
			if (!tRes.IsSuccess) {
				return new MActionResult<User>(tRes.Status);
			}
			var token = tRes.Content;
			if (token == null) {
				return new MActionResult<User>(StatusCode.InternalError);
			}

			var code = tokenMgr.GenerateCode(token);

			url += $"?code={code}";

			var mailMgr = Context.Get<MailManager>();
			var mRes = await mailMgr.ActivateAccountAsync(user, url);
			if (!mRes) {
				return new MActionResult<User>(StatusCode.JustALittleError);
			}

			return new MActionResult<User>(StatusCode.OK, user);
		}

		public Task<MActionResult<User>> ActivateAsync(int userId)
		{
			return this.DeOrActivateAsync(userId, true);
		}

		public Task<MActionResult<User>> DeactivateAsync(int userId)
		{
			return this.DeOrActivateAsync(userId, false);
		}

		private async Task<MActionResult<User>> DeOrActivateAsync(int userId, bool activate)
		{
			if (userId < 1) {
				return new MActionResult<User>(StatusCode.NotValidID);
			}
			var res = await this.GetByIdAsync(userId);
			if (!res.IsSuccess && res.Status != StatusCode.NotEnabled) {
				return res;
			}
			var user = res.Content;
			return await this.DeOrActivateAsync(user, activate);
		}

		private async Task<MActionResult<User>> DeOrActivateAsync(User u, bool activate)
		{
			if (u == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			if (u.Enabled == activate) {
				return new MActionResult<User>(StatusCode.OK, u);
			}
			u.Enabled = activate;
			await this.SaveAsync(u);
			return new MActionResult<User>(StatusCode.OK, u);
		}

#endregion

#region Other (Add XP, Set Mobile Token, ...)

		public async Task<bool> AddXpAsync(int howMuch, params int[] ids)
		{
			if (howMuch == 0) {
				return false;
			}
			foreach (int id in ids) {
				if (id < 1) {
					continue;
				}

				try {
					var user = await _ctx.Users.FindAsync(id);
					if (user != null) {
						user.XP += howMuch;
					}
				} catch {
#if DEBUG
					throw;
#else
					return false;
#endif
				}
			}
			await this.SaveAsync();
			return true;
		}

#region Mobile Token

		public Task<bool> ResetMobileTokenAsync(Guid uqid)
		{
			return SetMobileTokenAsync(uqid, null);
		}

		public async Task<bool> SetMobileTokenAsync(Guid uqid, string token)
		{
			if (uqid == Guid.Empty) {
				return false;
			}
			var res = await this.GetIdByGuidAsync(uqid);
			return await SetMobileTokenAsync(res, token);
		}

		private async Task<bool> SetMobileTokenAsync(int userId, string token)
		{
			if (userId < 1) {
				return false;
			}
			string key = SettingKeys.MobileToken;
			var setMgr = this.Context.Get<UserSettingManager>();

			if (string.IsNullOrWhiteSpace(token)) {
				return await setMgr.RemoveAsync(userId, key);
			}
			var res = await setMgr.CreateAsync(userId, key, token, true);
			return res.IsSuccess;
		}

#endregion

#endregion

#endregion

#endregion
	}
}