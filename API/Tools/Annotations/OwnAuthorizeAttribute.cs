using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using SharedLibrary.Shared;

namespace API.Tools.Annotations
{
	public class OwnAuthorizeAttribute : CustomAuthorizeAttribute
	{
		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			if (HttpContext.Current?.User?.Identity != null &&
				HttpContext.Current.User.Identity is ClaimsIdentity identity) {
				/*
				var a = identity.RoleClaimType;
				var b = identity.Claims;
				var c = identity.Claims.ToList();
				var d = identity.Claims.Where(x => x.Type == ClaimTypes.Role);
				var e = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
				*/
				var roles = identity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
				foreach (var f in roles) {
					if (!string.IsNullOrWhiteSpace(f)) {
						if (string.Equals(f, UserRoles.LoginOnly, StringComparison.CurrentCultureIgnoreCase)) {
							return false;
						}
					}
				}
			}
			return base.IsAuthorized(actionContext);
		}

		protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			if (!HttpContext.Current.User.Identity.IsAuthenticated) {
				base.HandleUnauthorizedRequest(actionContext);
			} else {
				actionContext.Response = GetForbiddenMessage(actionContext);
			}
		}
	}
}