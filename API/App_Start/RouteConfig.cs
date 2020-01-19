using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Swagger.Net.Application;

namespace API
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
							name: "Default",
							url: "{controller}/{action}/{id}",
							defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional}
						   );

			// Set Swagger portal as a home page.
			routes.MapHttpRoute(
								name: "Swagger_root",
								defaults: null,
								constraints: null,
								routeTemplate: string.Empty,
								handler: new RedirectHandler(m => $"{m.RequestUri}", "swagger")
							   );
		}
    }
}
