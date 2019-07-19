using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models.Admin
{
	public class DashboardModel
	{
#region School

		public int SchoolStudentsCount { get; set; }
		public int SchoolTeachersCount { get; set; }

#endregion

#region Global

		public int StudentsCount { get; set; }
		public int TeachersCount { get; set; }

#endregion

		public SendNotificationModel SendNot { get; set; }
	}
}