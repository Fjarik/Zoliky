using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ZolikyWeb.Startup))]

namespace ZolikyWeb
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigurateAuth(app);
		}
	}
}