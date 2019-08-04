using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models.Class
{
	public class ClassDashboardModel
	{
		public bool OnlyActive { get; set; }
		public IEnumerable<DataAccess.Models.Class> Classes { get; set; }

		public ClassDashboardModel()
		{
			this.Classes = new List<DataAccess.Models.Class>();
		}

		public ClassDashboardModel(IEnumerable<DataAccess.Models.Class> classes, bool onlyActive) : this()
		{
			this.Classes = classes;
			this.OnlyActive = onlyActive;
		}
	}
}