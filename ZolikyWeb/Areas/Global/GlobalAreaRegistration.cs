using System.Web.Mvc;

namespace ZolikyWeb.Areas.Global
{
	public class GlobalAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Global";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
							 "Global_default",
							 "Global/{controller}/{action}/{id}",
							 new {
								 controller = "Admin",
								 action = "Dashboard",
								 id = UrlParameter.Optional
							 },
							 namespaces: new[] {
								 "ZolikyWeb.Areas.Global.Controllers"
							 }
							);
		}
	}
}