using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models.Zoliky
{
	public class ZolikDashboardModel
	{
		public bool OnlyEnabled { get; set; }
		public IEnumerable<DataAccess.Models.Zolik> Zoliks { get; set; }

		public ZolikDashboardModel()
		{
			this.Zoliks = new List<DataAccess.Models.Zolik>();
		}

		public ZolikDashboardModel(IEnumerable<DataAccess.Models.Zolik> zoliks, bool onlyEnabled) : this()
		{
			this.Zoliks = zoliks;
			this.OnlyEnabled = onlyEnabled;
		}
	}
}