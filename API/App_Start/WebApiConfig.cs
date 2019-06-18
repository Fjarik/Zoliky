using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Web.Http;
using Microsoft.Web.Http.Routing;
using Microsoft.Web.Http.Versioning;

namespace API
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			config.AddApiVersioning(
									o => {
										o.DefaultApiVersion = new ApiVersion(2, 0);
										o.AssumeDefaultVersionWhenUnspecified = true;
										o.ReportApiVersions = true;
										o.ApiVersionReader = new HeaderApiVersionReader("api-version", "version");
									}
								   );

			var constraintResolver = new DefaultInlineConstraintResolver() {
				ConstraintMap = {
					["apiVersion"] = typeof(ApiVersionRouteConstraint)
				}
			};

			// Web API routes
			config.MapHttpAttributeRoutes(constraintResolver);

			config.Routes.MapHttpRoute(
									   name: "DefaultApi",
									   routeTemplate: "{controller}/{id}",
									   defaults: new {id = RouteParameter.Optional}
									  );
		}
	}
}