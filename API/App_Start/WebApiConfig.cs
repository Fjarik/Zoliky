using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Microsoft.Web.Http;
using Microsoft.Web.Http.Description;
using Microsoft.Web.Http.Routing;
using Microsoft.Web.Http.Versioning;
using Swagger.Net.Application;

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


			// Register Swagger service.
			config.RegisterSwagger(config.RegisterApiExplorer());
		}

		private static void RegisterSwagger(this HttpConfiguration configuration, VersionedApiExplorer apiExplorer)
		{
			configuration.EnableSwagger(
										"{apiVersion}/swagger/docs",
										swagger => {
											swagger.MultipleApiVersions(
																		(apiDescription, version) =>
																			apiDescription.GetGroupName() == version,
																		info => {
																			foreach (var group in apiExplorer
																				.ApiDescriptions) {
																				var description =
																					"Zoliky API description.";

																				if (group.IsDeprecated) {
																					description +=
																						" This API version has been deprecated.";
																				}

																				info.Version(group.Name,
																							 $"Zoliky API {group.ApiVersion}")
																					.Contact(c => c.Name("Jiři Falta")
																								   .Email("jirkafalta@gmail.com"))
																					.License(l => l.Name("MIT")
																								   .Url("https://opensource.org/licenses/MIT"))
																					.Description(description)
																					.TermsOfService("Shareware");
																			}
																		});
											swagger.PrettyPrint();
											swagger.DescribeAllEnumsAsStrings();
											swagger.Schemes(new[] {"http", "https"});
											swagger.IncludeAllXmlComments(typeof(WebApiConfig).Assembly,
																		  AppDomain.CurrentDomain.BaseDirectory);
										})
						 .EnableSwaggerUi(swagger => {
							 swagger.DocumentTitle("Simple Web API");
							 swagger.CssTheme("theme-flattop-css");
							 swagger.EnableDiscoveryUrlSelector();
						 });
		}

		private static VersionedApiExplorer RegisterApiExplorer(this HttpConfiguration configuration)
		{
			return configuration.AddVersionedApiExplorer(x => {
				x.GroupNameFormat = string.Empty;
				x.SubstituteApiVersionInUrl = true;
			});
		}
	}
}