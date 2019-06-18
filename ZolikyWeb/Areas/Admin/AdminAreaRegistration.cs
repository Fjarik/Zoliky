using System.Web.Mvc;

namespace ZolikyWeb.Areas.Admin
{
	public class AdminAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Admin";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
							 "Admin_default",
							 "Admin/{controller}/{action}/{id}",
							 new {
								 controller = "Admin",
								 action = "Dashboard",
								 id = UrlParameter.Optional
							 },
							 namespaces: new[] {
								 "ZolikyWeb.Areas.Admin.Controllers"
							 }
							);
		}
	}
}