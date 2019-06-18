using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ZolikyWeb
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

#region Account routes

			routes.MapRoute(name: "ForgetPwd",
							url: "forgetpwd",
							defaults: new {
								controller = "Home",
								action = "ForgetPwd",
							},
							namespaces: new[] {
								"ZolikyWeb.Controllers"
							}
						   );

			routes.MapRoute(name: "ForgotPwd",
							url: "forgotpwd",
							defaults: new {
								controller = "Home",
								action = "ForgetPwd",
							},
							namespaces: new[] {
								"ZolikyWeb.Controllers"
							}
						   );

			routes.MapRoute(name: "Login",
							url: "login",
							defaults: new {
								controller = "Home",
								action = "Login",
							},
							namespaces: new[] {
								"ZolikyWeb.Controllers"
							}
						   );

			routes.MapRoute(name: "Register",
							url: "register",
							defaults: new {
								controller = "Home",
								action = "Register",
							},
							namespaces: new[] {
								"ZolikyWeb.Controllers"
							}
						   );

#endregion

			routes.MapRoute(name: "Default",
							url: "{controller}/{action}/{id}",
							defaults: new {
								controller = "Home",
								action = "Index",
								id = UrlParameter.Optional
							},
							namespaces: new[] {
								"ZolikyWeb.Controllers"
							}
						   );
		}
	}
}