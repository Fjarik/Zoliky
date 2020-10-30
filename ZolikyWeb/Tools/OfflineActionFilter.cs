using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DataAccess;
using SharedLibrary.Shared;
using ZolikyWeb.Controllers;

namespace ZolikyWeb.Tools
{
	public class OfflineActionFilter : ActionFilterAttribute
	{
		public bool Active { get; set; } = true;

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!this.Active) {
				base.OnActionExecuting(filterContext);
				return;
			}
			var user = HttpContext.Current?.User;
			var isAuthorized = user?.Identity?.IsAuthenticated == true;
			if (isAuthorized) {
				var isAdmin = user.IsInRolesOr(UserRoles.Administrator, UserRoles.Developer);
				if (isAdmin) {
					base.OnActionExecuting(filterContext);
					return;
				}
			}
			var uri = filterContext.HttpContext?.Request?.Url;
			var url = "";
			if (uri != null) {
				url += $"https://{uri.Host}:{uri.Port}";
			}
			var helper = new OfflineHelper(url);
			helper.Init();
			if (helper.IsDown) {
				if (filterContext.IsChildAction) {
					filterContext.Result = new ContentResult {
						Content = string.Empty
					};
					return;
				}
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {
					Controller = "Home", Action = "Offline"
				}));
				var response = filterContext.HttpContext.Response;
				response.StatusCode = (int) HttpStatusCode.ServiceUnavailable;
				response.TrySkipIisCustomErrors = true;
				return;
			}
			base.OnActionExecuting(filterContext);
		}
	}
}