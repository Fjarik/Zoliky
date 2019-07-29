using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DataAccess.Managers;
using Microsoft.AspNet.Identity.Owin;
using DataAccess.Models;
using JetBrains.Annotations;
using Owin;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace DataAccess
{
	public static class Extensions
	{
		public static void RegisterManagers(this Owin.IAppBuilder app)
		{
			app.CreatePerOwinContext(ZoliksEntities.Create);

			app.CreatePerOwinContext<AchievementManager>(AchievementManager.Create);
			app.CreatePerOwinContext<BanManager>(BanManager.Create);
			app.CreatePerOwinContext<ClassManager>(ClassManager.Create);
			app.CreatePerOwinContext<HashManager>(HashManager.Create);
			app.CreatePerOwinContext<ImageManager>(ImageManager.Create);
			app.CreatePerOwinContext<LoginTokenManager>(LoginTokenManager.Create);
			app.CreatePerOwinContext<NewsManager>(NewsManager.Create);
			app.CreatePerOwinContext<NotificationManager>(NotificationManager.Create);
			app.CreatePerOwinContext<ProjectManager>(ProjectManager.Create);
			app.CreatePerOwinContext<ProjectSettingManager>(ProjectSettingManager.Create);
			app.CreatePerOwinContext<RankManager>(RankManager.Create);
			app.CreatePerOwinContext<SubjectManager>(SubjectManager.Create);
			app.CreatePerOwinContext<TokenManager>(TokenManager.Create);
			app.CreatePerOwinContext<TransactionManager>(TransactionManager.Create);
			app.CreatePerOwinContext<UnavailabilityManager>(UnavailabilityManager.Create);
			app.CreatePerOwinContext<UserLoginManager>(UserLoginManager.Create);
			app.CreatePerOwinContext<UserLogManager>(UserLogManager.Create);
			app.CreatePerOwinContext<UserManager>(UserManager.Create);
			app.CreatePerOwinContext<UserSettingManager>(UserSettingManager.Create);
			app.CreatePerOwinContext<ZolikManager>(ZolikManager.Create);

			app.CreatePerOwinContext<FirebaseManager>(FirebaseManager.Create);
			app.CreatePerOwinContext<MailManager>(MailManager.Create);
		}

#region IP Address

		public static string GetIPAddress(this System.Web.HttpRequestBase request)
		{
			string ip = request.UserHostAddress;
			if (string.IsNullOrWhiteSpace(ip) || ip == "::1") {
				ip = "127.0.0.1";
			}

			return ip;
		}

		public static string GetIPAddress(this System.Web.HttpContext context)
		{
			return DGlobals.GetIPAddress(context);
		}

		public static string GetIPAddress([CanBeNull] this System.Net.Http.HttpRequestMessage msg)
		{
			return DGlobals.GetIPAddress(msg);
		}

#endregion

#region Get logged user

		public static async Task<User> GetLoggedUserAsync(this System.Security.Claims.ClaimsPrincipal principal)
		{
			return await GetLoggedUserAsync(principal, HttpContext.Current.GetOwinContext().Get<UserManager>());
		}

		public static async Task<User> GetLoggedUserAsync(this System.Security.Claims.ClaimsPrincipal principal,
														  UserManager mgr)
		{
			if (mgr == null) {
				return null;
			}

			var g = GetLoggedUserGuid(principal);
			if (g == Guid.Empty) {
				return null;
			}

			var res = await mgr.SelectAsync(g);
			return res.IsSuccess ? res.Content : null;
		}

		public static Guid GetLoggedUserGuid(this System.Security.Claims.ClaimsPrincipal principal)
		{
			if (principal.Claims == null || principal.Claims.All(x => x.Type != ClaimTypes.NameIdentifier)) {
				return Guid.Empty;
			}

			var uqid = principal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier);
			if (!Guid.TryParse(uqid.Value, out Guid g)) {
				return Guid.Empty;
			}

			return g;
		}

#endregion

		public static TZolik Latest<TZolik>(this List<TZolik> list) where TZolik : class, IZolik
		{
			if (list == null || list.Count < 1) {
				return null;
			}

			if (list.Count == 1) {
				return list.First();
			}

			return list.OrderByDescending(x => x.OwnerSince).FirstOrDefault();
		}

#region Expressions

		public static Expression<Func<User, bool>> HasTrueSetting(string key, int? projectId = null)
		{
			return HasSetting(key, true.ToString());
		}

		public static Expression<Func<User, bool>> HasSetting(string key, string value, int? projectId = null)
		{
			return x => x.UserSettings.Any(s =>
											   s.Key == key && 
											   s.Value.ToLower() == value.ToLower() &&
											   s.ProjectID == projectId);
		}

		public static Expression<Func<Zolik, bool>> NonTesterZoliks()
		{
			return x => x.Type != ZolikType.Debug && x.Type != ZolikType.DebugJoker;
		}

		public static Expression<Func<Ban, bool>> IsActive()
		{
			return x => x.To == null || (x.To != null && x.To > DateTime.Now);
		}

		public static Expression<Func<Notification, bool>> IsNotExpired()
		{
			return x => x.Expiration == null || (x.Expiration != null && x.Expiration > DateTime.Now);
		}

#endregion

#region Authentication

		public static bool IsInRolesOr(this System.Security.Principal.IPrincipal principal, params string[] roles)
		{
			foreach (var role in roles) {
				if (principal.IsInRole(role)) {
					return true;
				}
			}
			return false;
		}

		public static bool IsInRolesAnd(this System.Security.Principal.IPrincipal principal, params string[] roles)
		{
			foreach (var role in roles) {
				if (!principal.IsInRole(role)) {
					return false;
				}
			}

			return true;
		}

		public static string GetClassName(this System.Security.Principal.IIdentity identity)
		{
			return GetValue<string>(identity, "className");
		}

		public static string GetSchoolName(this System.Security.Principal.IIdentity identity)
		{
			return GetValue<string>(identity, "schoolName");
		}

		public static int GetId(this System.Security.Principal.IIdentity identity)
		{
			return GetValue<int>(identity, "publicId");
		}

		public static bool IsTester(this System.Security.Principal.IIdentity identity)
		{
			return GetValue<bool>(identity, "isTester");
		}

		private static T GetValue<T>(this System.Security.Principal.IIdentity identity, string key)
			where T : IConvertible
		{
			if (!(identity is ClaimsIdentity claims)) {
				return default;
			}
			var claim = claims.FindFirst(key);
			if (claim == null) {
				return default;
			}
			return (T) Convert.ChangeType(claim.Value, typeof(T));
		}

#endregion

#region Files

		public static bool IsImage(this System.Web.HttpPostedFile file)
		{
			if (file == null || string.IsNullOrWhiteSpace(file.ContentType)) {
				return false;
			}

			string type = file.ContentType.ToLower();
			return CheckType(type);
		}

		public static bool IsImage(this System.Web.HttpPostedFileBase file)
		{
			if (file == null || string.IsNullOrWhiteSpace(file.ContentType)) {
				return false;
			}

			string type = file.ContentType.ToLower();
			return CheckType(type);
		}

		private static bool CheckType(string type)
		{
			if (!type.Contains("image/")) {
				return false;
			}

			type = type.Substring(6);

			switch (type) {
				case "gif":
				case "jpeg":
				case "png":
				case "webp":
					return true;
				default:
					return false;
			}
		}

#endregion
	}
}