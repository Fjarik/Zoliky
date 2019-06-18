using System.Web.Mvc;

namespace ZolikyWeb.Areas.App
{
	public class AppAreaRegistration : AreaRegistration
	{
		public override string AreaName => "App";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
							 "App_default",
							 "App/{controller}/{action}/{id}",
							 new {
								 controller = "Main",
								 action = "Dashboard",
								 id = UrlParameter.Optional
							 },
							 namespaces: new[] {
								 "ZolikyWeb.Areas.App.Controllers"
							 }
							);
		}
	}
}