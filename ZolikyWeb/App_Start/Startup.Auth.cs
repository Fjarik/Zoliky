using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.MicrosoftAccount;
using Owin;
using ZolikyWeb.Tools;

namespace ZolikyWeb
{
	public partial class Startup
	{
		private void ConfigurateAuth(IAppBuilder app)
		{
			app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

			//System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

#region Managers

			app.RegisterManagers();

			app.CreatePerOwinContext<SignInManager>(SignInManager.Create);

#endregion

#region Authentication

			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
			app.UseCookieAuthentication(new CookieAuthenticationOptions {
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login"),
				LogoutPath = new PathString("/Account/LogOff"),
				ReturnUrlParameter = "r",
				CookieSecure = CookieSecureOption.SameAsRequest,
			});
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
			app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

			// Enables the application to remember the second login verification factor such as phone or email.
			// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
			// This is similar to the RememberMe option when you log in.
			app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

#endregion

#region Routing and Optimization

			// Code that runs on application startup
			AreaRegistration.RegisterAllAreas();

			// Routing
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			// Optimization
			BundleConfig.RegisterBundles(BundleTable.Bundles);

#endregion

#region Filters

			GlobalFilters.Filters.Add(new OfflineActionFilter());

#endregion

#region External logins

			app.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions() {
				ClientId = "e71095dd-09e4-4c72-bae1-9ce63fa9d10a",
				ClientSecret = "a!}!).+Bg{j>!@#M|:$-9U/$;"
			});

			app.UseFacebookAuthentication(new FacebookAuthenticationOptions() {
				AppId = "2264696540486434",
				AppSecret = "a0a7042ba5455639b221b2e0b8f0aa50"
			});

			app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions() {
				ClientId = "841578341360-ol0dlo8buura5to7tpmu4pk2vjapa5e9.apps.googleusercontent.com",
				ClientSecret = "yH-x8i7exRJjkImNnYEVU6vf"
			});

#endregion
		}
	}
}