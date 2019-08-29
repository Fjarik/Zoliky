using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ZolikyWeb
{
	public static class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			RegisterStyleBundles(bundles);

			RegisterScriptBundles(bundles);

			bundles.UseCdn = true;
			BundleTable.EnableOptimizations = true;
		}

		private static void RegisterStyleBundles(BundleCollection bundles)
		{
#region Bootstrap

			bundles.Add(new StyleBundle("~/bundles/css/bootstrap")

							// Bootstrap
							//.Include("~/Content/bootstrap.css")
							//.Include("~/Content/bootstrap.min.css")

							// Material Bootstrap
							.Include("~/Content/material.css")
						//.Include("~/Content/material.min.css")
					   );

#endregion

#region Main styles

			bundles.Add(new StyleBundle("~/bundles/css/main")
						// JQuery Ui
						.Include("~/Content/themes/base/jquery-ui.min.css")
						//.Include("~/Content/themes/base/all.css")
						.Include("~/Content/themes/base/theme.min.css")

						// Bootstrap Social buttons
						.Include("~/Content/bootstrap-social.min.css")

						// Toastr
						.Include("~/Content/toastr.min.css")

						// LightBox
						.Include("~/Content/lightbox.min.css", new CssRewriteUrlTransform())

						// Own Styles
						.Include("~/Content/Main.min.css")
					   );

#endregion

#region Main page

			bundles.Add(new StyleBundle("~/bundles/css/mainpage")
						.Include("~/Content/MainPage/owl.carousel.min.css")
						.Include("~/Content/MainPage/slicknav.min.css")
						.Include("~/Content/MainPage/typography.min.css")
						.Include("~/Content/MainPage/style.min.css")
						.Include("~/Content/MainPage/responsive.min.css")
					   );

#endregion

#region Account styles

			bundles.Add(new StyleBundle("~/bundles/css/account")
							.Include("~/Content/Account.css")
					   );

#endregion

#region Dashboard

			bundles.Add(new StyleBundle("~/bundles/css/dashboard")
							.IncludeDirectory("~/Content/Dashboard/", "*.css", true));

#endregion
		}

		private static void RegisterScriptBundles(BundleCollection bundles)
		{
#region JQuery

			string cdn = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.4.1.min.js";
			bundles.Add(new ScriptBundle("~/bundles/js/jquery", cdn)
							.Include("~/Scripts/jquery-3.4.1.min.js")
					   );

#endregion

#region Main scripts

			bundles.Add(new ScriptBundle("~/bundles/js/main")
						// JQuery Validate
						.Include("~/Scripts/jquery.validate*")

						// JQuery UI
						.Include("~/Scripts/jquery-ui-1.12.1.js")
						.Include("~/Scripts/jquery-ui-1.12.1.min.js")

						// Modernizr
						//.Include("~/Scripts/modernizr-*")

						// FontAwesome
						.Include("~/Scripts/fa-all.js")
						.Include("~/Scripts/fa-all.min.js")

						// Popper
						.Include("~/Scripts/umd/popper.js")
						.Include("~/Scripts/umd/popper.min.js")

						// Bootstrap
						.Include("~/Scripts/bootstrap.js")
						.Include("~/Scripts/bootstrap.min.js")

						// Material Bootstrap
						.Include("~/Scripts/material.js")
						.Include("~/Scripts/material.min.js")

						// Toastr
						.Include("~/Scripts/toastr.min.js")

						// Cookies
						.Include("~/Scripts/js-cookie/js.cookie-2.2.1.min.js")

						// LightBox
						.Include("~/Scripts/lightbox.min.js")

						// Own scripts
						.Include("~/Scripts/main.min.js")
					   );

#endregion

#region Main page

			bundles.Add(new ScriptBundle("~/bundles/js/mainpage")
						.Include("~/Scripts/MainPage/jquery.slicknav.min.js")
						.Include("~/Scripts/MainPage/owl.carousel.min.js")
						.Include("~/Scripts/MainPage/counterup.min.js")
						.Include("~/Scripts/MainPage/jquery.waypoints.min.js")
						.Include("~/Scripts/MainPage/theme.min.js")
					   );

#endregion

#region Account

			bundles.Add(new ScriptBundle("~/bundles/js/account")
							.Include("~/Scripts/account.js"));

#endregion

#region Dashboard

			bundles.Add(new ScriptBundle("~/bundles/js/dashboard")
						.IncludeDirectory("~/Scripts/Dashboard/", "*.js", true)
						.Include("~/Scripts/dashboard.js"));

#endregion
		}
	}
}