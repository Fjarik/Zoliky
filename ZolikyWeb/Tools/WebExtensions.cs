using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace ZolikyWeb.Tools
{
	public static class WebExtensions
	{
#region Authentication

		public static bool IsAuthenticated(this Controller controller)
		{
			return controller.User?.Identity != null && controller.User.Identity.IsAuthenticated;
		}

		public static async Task<User> GetLoggedUserAsync(this Controller controller)
		{
			if (!controller.IsAuthenticated() || string.IsNullOrWhiteSpace(controller.User.Identity.GetUserId())) {
				return null;
			}

			var uqid = controller.User.Identity.GetUserId();

			UserManager mgr = controller.GetManager<UserManager>();
			var res = await mgr.SelectAsync(uqid, WhatToCheck.Guid);
			if (!res.IsSuccess) {
				return null;
			}

			return res.Content;
		}
		
		public static int GetSchoolId(this System.Security.Principal.IPrincipal principal)
		{
			if (principal.IsInRolesOr(UserRoles.Administrator, UserRoles.Developer) &&
				HttpContext.Current.Session[Ext.Session.SchoolID] is int schoolId) {
				return schoolId;
			}
			return principal.Identity.GetValue<int>("schoolId");
		}

#endregion

#region Entities

		public static TManager GetManager<TManager>(this Controller controller) where TManager : class, IDisposable
		{
			return controller.HttpContext.GetOwinContext().Get<TManager>();
		}

#endregion

#region Toastr extensions

		public static ToastMessage AddToastMessage(this Controller controller,
												   string title,
												   string message,
												   ToastType type)
		{
			if (!(controller.TempData["Toastr"] is Toastr toastr)) {
				toastr = new Toastr();
			}

			var toastMessage = toastr.AddToastMessage(title, message, type);
			controller.TempData["Toastr"] = toastr;
			return toastMessage;
		}

		public static ToastMessage AddSuccessToastMessage(this Controller controller, string message)
		{
			return AddToastMessage(controller, "Úspěch", message, ToastType.Success);
		}

		public static ToastMessage AddWarningToastMessage(this Controller controller, string message)
		{
			return AddToastMessage(controller, "Varování", message, ToastType.Warning);
		}

		public static ToastMessage AddErrorToastMessage(this Controller controller, string message)
		{
			return AddToastMessage(controller, "Chyba", message, ToastType.Error);
		}

#endregion

#region ProfilePhoto

		public static string GetProfilePhotoUrl(this UrlHelper urlHelper, int userId)
		{
			var cacheBurst = 0;
			if (HttpContext.Current.Session[Globals.CacheBurst] is int i) {
				cacheBurst = i;
			}
			var url = urlHelper.Action("ProfilePhoto", "Account", new {userId = userId, c = cacheBurst, Area = ""});
			return url;
		}

		public static string GetCurrentUserProfilePhotoUrl(this UrlHelper urlHelper)
		{
			var userId = HttpContext.Current.User.Identity.GetId();
			return GetProfilePhotoUrl(urlHelper, userId);
		}

#endregion

#region Roles

		public static string GetColor(this Role r)
		{
			var rName = r.Name;
			var lbl = "primary";
			switch (rName) {
				case UserRoles.Teacher:
					lbl = "danger";
					break;
				case UserRoles.Developer:
				case UserRoles.Administrator:
					lbl = "warning";
					break;
				case UserRoles.Tester:
					lbl = "success";
					break;
				case UserRoles.Public:
					lbl = "info";
					break;
			}
			return lbl;
		}

#endregion

#region Url

		///
		/// https://stackoverflow.com/questions/21932635/how-to-add-a-parameter-to-the-current-url-in-a-view
		/// 
		private static RouteValueDictionary GetKeyValuePairs(this UrlHelper urlHelper)
		{
			var routeValueDictionary = new RouteValueDictionary(urlHelper.RequestContext.RouteData.Values);
			var queryString = HttpContext.Current.Request.QueryString;
			foreach (var key in queryString.AllKeys)
				routeValueDictionary.Add(key, queryString[key]);

			return routeValueDictionary;
		}

		public static string BuildUrl(this UrlHelper urlHelper)
		{
			var routeValueDictionary = GetKeyValuePairs(urlHelper);
			return urlHelper.RouteUrl(routeValueDictionary);
		}

		public static string BuildUrl(this UrlHelper urlHelper, string key)
		{
			var routeValueDictionary = GetKeyValuePairs(urlHelper);

			if (routeValueDictionary.Keys.Contains(key))
				routeValueDictionary.Remove(key);

			return urlHelper.RouteUrl(routeValueDictionary);
		}

		public static string BuildUrl(this UrlHelper urlHelper, string key, object value)
		{
			var routeValueDictionary = GetKeyValuePairs(urlHelper);
			if (routeValueDictionary.ContainsKey(key))
				routeValueDictionary[key] = value;
			else
				routeValueDictionary.Add(key, value);

			return urlHelper.RouteUrl(routeValueDictionary);
		}

#endregion
	}
}