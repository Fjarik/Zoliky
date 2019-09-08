using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Global.Models.Subject
{
	public class SubjectDashboardModel
	{
		public List<DataAccess.Models.Subject> Subjects { get; set; }

		public SubjectDashboardModel() : this(new List<DataAccess.Models.Subject>())
		{
		}

		public SubjectDashboardModel(List<DataAccess.Models.Subject> subjects)
		{
			this.Subjects = subjects;
		}
	}
}