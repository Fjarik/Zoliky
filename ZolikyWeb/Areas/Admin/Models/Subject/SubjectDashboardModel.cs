using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models.Subject
{
	public class SubjectDashboardModel
	{
		public List<DataAccess.Models.Subject> Subjects { get; set; }
		public bool OnlyAssigned { get; set; }

		public SubjectDashboardModel()
		{
			Subjects = new List<DataAccess.Models.Subject>();
		}

		public SubjectDashboardModel(List<DataAccess.Models.Subject> subjects, bool onlyAssigned) : this()
		{
			this.Subjects = subjects;
			this.OnlyAssigned = onlyAssigned;
		}

	}
}