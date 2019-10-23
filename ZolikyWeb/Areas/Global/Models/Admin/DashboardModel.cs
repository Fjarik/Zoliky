using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZolikyWeb.Areas.Global.Models.Admin.Dashboard;

namespace ZolikyWeb.Areas.Global.Models.Admin
{
	public class DashboardModel
	{
#region Global

		public int ZoliksCount { get; set; }
		public int SchoolsCount { get; set; }
		public int StudentsCount { get; set; }
		public int TeachersCount { get; set; }

#endregion

		public SendMobileNotModel SendMobileNot { get; set; }
		public SendNotificationsModel SendNotifications { get; set; }

		public DashboardModel(int defaultToId)
		{
			this.SendMobileNot = new SendMobileNotModel {
				ToId = defaultToId
			};
			this.SendNotifications = new SendNotificationsModel {
				ToID = defaultToId
			};
		}
	}
}