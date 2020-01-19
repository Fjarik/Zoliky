using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using API.Tools;
using DataAccess;
using DataAccess.Managers.New;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace API
{
	public partial class Startup
	{
		private void ConfigureAuth(IAppBuilder app)
		{
			app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

#region Managers

			app.RegisterManagers();

#endregion

#region Authentication

			app.UseCookieAuthentication(new CookieAuthenticationOptions() {
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				//LoginPath = new PathString("/User/Login"),
				//LogoutPath = new PathString("/User/Logout"),
				ReturnUrlParameter = "r",
				CookieSecure = CookieSecureOption.SameAsRequest,
			});

#endregion

#region Bearer token

			OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions() {
#if DEBUG
				AllowInsecureHttp = true,
#else
				AllowInsecureHttp = false,
#endif
				TokenEndpointPath = new PathString("/Token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
				Provider = new AuthorizationServerProvider(),
			};
			app.UseOAuthAuthorizationServer(options);
			app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

#endregion

#region Http config

			HttpConfiguration config = new HttpConfiguration();
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			WebApiConfig.Register(config);

			#endregion
		}
	}
}