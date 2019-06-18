using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using DataAccess.Errors;
using DataAccess.Models;
using Flurl.Util;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class UserManager : IManager<User>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		public UserManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		public MActionResult<User> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<User>(StatusCode.NotValidID);
			}
			User u = _ent.Users
						 .Include(x => x.Class)
						 .Include(x => x.Roles)
						 .AsNoTracking()
						 .FirstOrDefault(x => x.ID == id);
			if (u == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			if (!u.IsEnabled) {
				return new MActionResult<User>(StatusCode.NotEnabled, u);
			}

			//u.Password = "";
			return new MActionResult<User>(StatusCode.OK, u);
		}

		[NotNull]
		public MActionResult<User> Select([NotNull] Guid g, bool tester)
		{
			User u = _ent.Users.AsNoTracking().FirstOrDefault(x => x.UQID == g);
			if (u == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			if (!u.IsEnabled) {
				return new MActionResult<User>(StatusCode.NotEnabled, u);
			}

			if (tester) {
				u.IsTesterType = true;
			}

			//u.Password = "";
			return new MActionResult<User>(StatusCode.OK, u);
		}

		[NotNull]
		public MActionResult<User> Select([NotNull] string uname)
		{
			if (string.IsNullOrWhiteSpace(uname)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			uname = uname.Trim().ToLower();

			if (uname.Contains("petatrunda")) {
				uname = uname.Replace("petatrunda", "zerzpe");
			}

			bool isTester = false;
			if (uname.Contains("_tester")) {
				uname = uname.Replace("_tester", "");
				isTester = true;
			}

			User u = null;
			if (uname.Contains('@')) {
				u = _ent.Users.AsNoTracking().FirstOrDefault(x => x.Email.ToLower() == uname);
			} else {
				u = _ent.Users.AsNoTracking().FirstOrDefault(x => x.Username.ToLower() == uname);
			}
			if (u == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			if (!u.IsEnabled) {
				return new MActionResult<User>(StatusCode.NotEnabled, u);
			}
			if (isTester) {
				if (!u.HasTesterRights) {
					return new MActionResult<User>(StatusCode.InsufficientPermissions);
				}
				u.IsTesterType = true;
			}

			//u.Password = "";
			return new MActionResult<User>(StatusCode.OK, u);
		}

		[NotNull]
		public MActionResult<User> Login([NotNull] string uName, [NotNull] string password, Projects project,
										 [CanBeNull] string ip)
		{
			if (string.IsNullOrWhiteSpace(uName) || string.IsNullOrWhiteSpace(password)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			uName = uName.Trim().ToLower();

			if (uName.Contains("petatrunda")) {
				uName = uName.Replace("petatrunda", "zerzpe");
			}

			bool isTester = false;
			if (uName.Contains("_tester")) {
				uName = uName.Replace("_tester", "");
				isTester = true;
			}

			User u = _ent.Users.AsNoTracking().SingleOrDefault(x => x.Username.Trim().ToLower() == uName);
			if (u == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			SomeHash pwdHash = u.Password;
			if (pwdHash == null) {
				return new MActionResult<User>(StatusCode.NoPassword);
			}
			if (pwdHash.IsExpired) {
				return new MActionResult<User>(StatusCode.ExpiredPassword);
			}

			var res = VerifyHashedPassword(u.ID, pwdHash, password);
			switch (res) {
				case PasswordVerificationResult.Failed:
					_mgr.Logins.Create(u.ID, project, LoginStatus.BadPass, ip);
					return new MActionResult<User>(StatusCode.WrongPassword);
				case PasswordVerificationResult.SuccessRehashNeeded:
					//TODO: Rehash
					return new MActionResult<User>(StatusCode.ExpiredPassword);
				case PasswordVerificationResult.Success:
					break;
				default:
					_mgr.Logins.Create(u.ID, project, LoginStatus.Error, ip);
					return new MActionResult<User>(StatusCode.InternalError);
			}

			if (!u.IsEnabled) {
				return new MActionResult<User>(StatusCode.NotEnabled, u);
			}

			if (isTester) {
				if (!u.HasTesterRights) {
					return new MActionResult<User>(StatusCode.InsufficientPermissions);
				}
				u.IsTesterType = true;
				_mgr.Logins.Create(u.ID, project, LoginStatus.Success, ip, "tester");
			} else {
				_mgr.Logins.Create(u.ID, project, LoginStatus.Success, ip);
			}

			//this.Save(u);
			return new MActionResult<User>(StatusCode.OK, u);
		}

		public PasswordVerificationResult VerifyHashedPassword(int userId, SomeHash hash, string password)
		{
			if (userId < 1 || hash == null || string.IsNullOrWhiteSpace(password)) {
				return PasswordVerificationResult.Failed;
			}
			if (hash.Version == "1.0.1") {
				// Without salt, password same
			}
			if (hash.Version == "1.0.2") {
				// With salt
				password = $"{userId}{password}{Convert.ToBase64String(hash.Salt)}{userId}";
			}
			var res = new PasswordHasher().VerifyHashedPassword(hash.Hash, password);
			return res;
		}

		public async Task<MActionResult<User>> LoginAsync([NotNull] string uName, [NotNull] string password)
		{
			if (string.IsNullOrWhiteSpace(uName) || string.IsNullOrWhiteSpace(password)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			uName = uName.Trim().ToLower();

			if (uName.Contains("petatrunda")) {
				uName = uName.Replace("petatrunda", "zerzpe");
			}

			bool isTester = false;
			if (uName.Contains("_tester")) {
				uName = uName.Replace("_tester", "");
				isTester = true;
			}

			User u = await _ent.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username.ToLower() == uName)
							   .ConfigureAwait(true);
			if (u == null) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			SomeHash pwdHash = u.Password;
			if (pwdHash == null) {
				return new MActionResult<User>(StatusCode.NoPassword);
			}
			if (pwdHash.IsExpired) {
				return new MActionResult<User>(StatusCode.ExpiredPassword);
			}

			var res = VerifyHashedPassword(u.ID, pwdHash, password);
			switch (res) {
				case PasswordVerificationResult.Failed:
					return new MActionResult<User>(StatusCode.WrongPassword);
				case PasswordVerificationResult.SuccessRehashNeeded:
					return new MActionResult<User>(StatusCode.ExpiredPassword);
				case PasswordVerificationResult.Success:
					break;
				default:
					return new MActionResult<User>(StatusCode.InternalError);
			}

			if (!u.IsEnabled) {
				return new MActionResult<User>(StatusCode.NotEnabled, u);
			}

			if (isTester) {
				if (!u.HasTesterRights) {
					return new MActionResult<User>(StatusCode.InsufficientPermissions);
				}
				u.IsTesterType = true;
			}

			await this.SaveAsync(u).ConfigureAwait(true);
			return new MActionResult<User>(StatusCode.OK, u);
		}

		[NotNull]
		public MActionResult<User> Register([NotNull] string uname, [NotNull] string password, [NotNull] string fname,
											[NotNull] string lname, [NotNull] Sex gender, int? classId,
											[NotNull] string email, [NotNull] string ip = "127.0.0.1",
											bool createWiki = false)
		{
			if (string.IsNullOrWhiteSpace(uname) || string.IsNullOrWhiteSpace(password) ||
				string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(lname) ||
				string.IsNullOrWhiteSpace(email)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			if (Exists(uname, WhatToCheck.Username) || Exists(email, WhatToCheck.Email)) {
				return new MActionResult<User>(StatusCode.AlreadyExists);
			}

			// Password Hashing
			string pwdhash = new PasswordHasher().HashPassword(password);


			// Create User
			User u = new User() {
				ClassID = classId,
				PasswordID = null,
				UQID = Guid.NewGuid(),
				Username = uname,
				Email = email,
				Name = fname,
				Lastname = lname,
				Type = 0,
				Enabled = false,
				MemberSince = DateTime.Now,
				RegistrationIp = ip,
				Description = "",
				XP = 0,
				ProfilePhotoID = 4,
				Sex = gender,
				VersionS = "1.0.1"
			};
			User u1 = _ent.Users.Add(u);
			Save(null);
			u1 = _ent.Users.FirstOrDefault(x => x.ID == u1.ID);

			if (u1 == null) {
				return new MActionResult<User>(StatusCode.InternalError);
			}

			// Create Hash in database
			var mAh = _mgr.Hashes.Create(pwdhash, u1.ID);
			if (!mAh.IsSuccess) {
				return new MActionResult<User>(mAh.Status);
			}

			// Add student role
			var role = _ent.Roles.FirstOrDefault(x => x.Name == UserRoles.Student);
			if (role != null && u1.Roles.All(x => x.Name != role.Name)) {
				u1.Roles.Add(role);
				Save(u1);
			}

			if (createWiki) {
				var wikiRes = new WikiConnector().CreateUser(u1.Username, password, u1.Email, u1.FullName);
				// TODO: Wiki result
			}

			// Detach user
			u1 = _ent.Users.AsNoTracking().FirstOrDefault(x => x.ID == u1.ID);
			if (u1 == null) {
				return new MActionResult<User>(StatusCode.InternalError);
			}
			return new MActionResult<User>(StatusCode.OK, u1);
		}

		public MActionResult<User> ChangePwd(int userId, int newHashId)
		{
			if (userId < 1 || newHashId < 1) {
				return new MActionResult<User>(StatusCode.NotValidID);
			}
			using (ZoliksEntities ctx = new ZoliksEntities()) {
				var user = ctx.Users.Find(userId);
				if (user != null) {
					user.PasswordID = newHashId;
					ctx.SaveChanges();
					return new MActionResult<User>(StatusCode.OK, user);
				}
			}
			return new MActionResult<User>(StatusCode.InternalError);
		}

		[NotNull]
		public MActionResult<User> Register([NotNull] RegisterUser reg)
		{
			if (reg == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}
			return Register(reg.UserName, reg.Pwd, reg.FirstName, reg.LastName, reg.Gender, reg.ClassID, reg.Email,
							reg.IP, reg.Wiki);
		}

		[NotNull]
		public MActionResult<User> Activate(int userId)
		{
			using (ZoliksEntities ctx = new ZoliksEntities()) {
				var user = ctx.Users.Find(userId);
				if (user == null || user.Enabled) {
					return new MActionResult<User>(StatusCode.NotFound);
				}
				user.Enabled = true;
				ctx.Entry(user).State = EntityState.Modified;
				ctx.SaveChanges();
				return new MActionResult<User>(StatusCode.OK, user);
			}
		}

		[NotNull]
		public MActionResult<List<User>> GetStudents(int? classId = null, bool onlyActive = true)
		{
			var query = _ent.Users.AsNoTracking()
							.Where(x => x.Type != UserPermission.Teacher &&
										x.Type != UserPermission.Dev &&
										x.ClassID != null);
			if (classId != null) {
				query = query.Where(x => x.ClassID == classId);
			}

			List<User> users = new List<User>();
			users.AddRange(query.OrderBy(x => x.Class.Name).ThenBy(x => x.Name).ThenBy(x => x.Lastname));
			//users.AddRange(_ent.Users.AsNoTracking().Where(x => x.Type != UserPermission.Admin && x.Type != UserPermission.Dev && x.ClassID != null).OrderBy(x => x.Class.Name).ThenBy(x => x.Name).ThenBy(x => x.Lastname).ToList());
			if (onlyActive) {
				users.RemoveAll(x => !x.IsEnabled);
			}

			return new MActionResult<List<User>>(StatusCode.OK, users);
		}

		[NotNull]
		public MActionResult<List<User>> GetBasicStudents(int excludeId, int? classId = null)
		{
			var smt = _ent.Users.AsNoTracking()
						  .Where(x => x.Type != UserPermission.Teacher && x.Type != UserPermission.Dev &&
									  x.ID != excludeId && x.ClassID != null);

			if (classId != null && classId > 1) {
				smt = smt.Where(x => x.ClassID == classId);
			}

			var res = smt
					  .OrderBy(x => x.Class.Name)
					  .ThenBy(x => x.Lastname)
					  .ThenBy(x => x.Name)
					  .Select(x => new {x.ID, x.Name, x.Lastname, x.ClassID, x.Class})
					  .ToList()
					  .Select(x => new User {
						  ID = x.ID, Name = x.Name, Lastname = x.Lastname, ClassID = x.ClassID, Class = x.Class
					  }).ToList();
			return new MActionResult<List<User>>(StatusCode.OK, res);
		}

		public int GetStudentsCount()
		{
			var res = this.GetStudents();
			if (!res.IsSuccess) {
				return 0;
			}
			return res.Content.Count;
		}

		public bool Delete(int userID)
		{
			try {
				var mAu = Select(userID);
				if (!mAu.IsSuccess && mAu.Status != StatusCode.NotEnabled) {
					return false;
				}
				User u = mAu.Content;
				if (u.Type != UserPermission.User) {
					return false;
				}
				u.PasswordID = null;
				_ent.Users.AddOrUpdate(u);
				_ent.SaveChanges();

#region Relations

				var passwords = _ent.SomeHashes.Where(x => x.OwnerID == userID);
				_ent.SomeHashes.RemoveRange(passwords);

				var consents = _ent.Consents.Where(x => x.UserID == userID);
				_ent.Consents.RemoveRange(consents);

				var tokens = _ent.Tokens.Where(x => x.UserID == userID);
				_ent.Tokens.RemoveRange(tokens);

				var logins = _ent.UserLogins.Where(x => x.UserID == userID);
				_ent.UserLogins.RemoveRange(logins);
				_ent.SaveChanges();

				var settings = _ent.UserSettings.Where(x => x.ID == userID);
				_ent.UserSettings.RemoveRange(settings);
				_ent.SaveChanges();

				var images = _ent.Images.Where(x => x.OwnerID == userID);
				_ent.Images.RemoveRange(images);
				_ent.SaveChanges();

				var crashes = _ent.Crashes.Where(x => x.UserID == userID);
				_ent.Crashes.RemoveRange(crashes);
				_ent.SaveChanges();

				var events = _ent.WebEvents.Where(x => x.FromID == userID || x.ToID == userID);
				_ent.WebEvents.RemoveRange(events);
				_ent.SaveChanges();

				var notifications = _ent.Notifications.Where(x => x.FromID == userID || x.ToID == userID);
				_ent.Notifications.RemoveRange(notifications);
				_ent.SaveChanges();

#endregion

				var originals = _ent.Zoliky.Where(x => x.OriginalOwnerID == userID);
				foreach (var zolik in originals) {
					zolik.OriginalOwnerID = (int) Teachers.Zerzan;
					if (zolik.Type == ZolikType.Debug) {
						zolik.OriginalOwnerID = (int) Teachers.Admin;
					}
					_ent.Entry(zolik).State = EntityState.Modified;
				}
				_ent.SaveChanges();

				foreach (var zolik in _ent.Zoliky.Where(x => x.OwnerID == userID)) {
					ZolikPackage zp = new ZolikPackage() {
						FromID = userID,
						ToID = (int) Teachers.Admin,
						ZolikID = zolik.ID,
						Type = TransactionAssignment.ZerziRemoval,
						Message = $"Žolík odebrán při odstraňování účtu ({u.FullName})"
					};
					var res = new Manager().Zoliky.Transfer(zp, new User() {ID = 0, Type = UserPermission.Dev});
					if (!res.IsSuccess) {
						return false;
					}
				}

				foreach (var tran in _ent.Transactions.Where(x => x.ToID == userID || x.FromID == userID)) {
					if (tran.FromID == userID) {
						tran.FromID = (int) Teachers.Admin;
					}
					if (tran.ToID == userID) {
						tran.ToID = (int) Teachers.Admin;
					}
					_ent.Entry(tran).State = EntityState.Modified;
				}
				_ent.SaveChanges();


				_ent = new ZoliksEntities();
				mAu = Select(userID);
				if (!mAu.IsSuccess && mAu.Status != StatusCode.NotEnabled) {
					return false;
				}
				u = mAu.Content;

				_ent.Users.Attach(u);
				_ent.Users.Remove(u);
				_ent.SaveChanges();

				return true;
			} catch {
#if DEBUG
				throw;
#endif
				return false;
			}
		}

		[NotNull]
		public bool Exists([NotNull] string input, WhatToCheck what)
		{
			if (string.IsNullOrWhiteSpace(input)) {
				return false;
			}

			input = input.Trim().ToLower();

			bool exists = false;

			switch (what) {
				case WhatToCheck.Username:
					exists = _ent.Users.Any(x => x.Username.ToLower() == input);
					break;
				case WhatToCheck.Email:
					exists = _ent.Users.Any(x => x.Email.ToLower() == input);
					break;
				default:
					return false;
			}

			return exists;
		}

		[NotNull]
		public bool Exists([NotNull] Guid g)
		{
			if (_ent.Users.Any(x => x.UQID == g)) {
				return true;
			}
			return false;
		}

		public bool Exists(int id) => _ent.Users.Any(x => x.ID == id);

		public bool CheckIdAndUqid(int id, string uqid)
		{
			if (id < 1 || string.IsNullOrWhiteSpace(uqid) || !Guid.TryParse(uqid, out Guid g)) {
				return false;
			}
			return _ent.Users.Any(x => x.ID == id && x.UQID == g);
		}

		public bool AddXp(int howMuch, params int[] ids)
		{
			if (howMuch == 0) {
				return false;
			}
			foreach (int id in ids) {
				if (id < 1) {
					continue;
				}

				try {
					var user = _ent.Users.Find(id);
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
			_ent.SaveChanges();
			return true;
		}

		public int Save(User u, bool throwException = false)
		{
			try {
				if (u != null) {
					if (_ent.Entry(u).State != EntityState.Unchanged) {
						_ent.Entry(u).State = EntityState.Modified;
					}
					_ent.Users.AddOrUpdate(u);
				}

				int changes = _ent.SaveChanges();
				return changes;
			} catch (DbEntityValidationException ex) {
				if (throwException) {
					throw new DbEntityValidationException(ex.GetExceptionMessage(), ex.EntityValidationErrors);
				}
				return 0;
			}
		}

		public async Task<int> SaveAsync(User u, bool throwException = false)
		{
			try {
				if (u != null) {
					_ent.Users.AddOrUpdate(u);
				}
				return await _ent.SaveChangesAsync();
			} catch (DbEntityValidationException ex) {
				if (throwException) {
					throw new DbEntityValidationException(ex.GetExceptionMessage(), ex.EntityValidationErrors);
				}
				return 0;
			}
		}
	}
}