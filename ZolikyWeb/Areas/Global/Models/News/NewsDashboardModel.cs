using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Global.Models.News
{
	public class NewsDashboardModel
	{
		public List<DataAccess.Models.News> News { get; set; }

		public NewsDashboardModel() : this(new List<DataAccess.Models.News>()) { }

		public NewsDashboardModel(List<DataAccess.Models.News> news)
		{
			News = news;
		}
	}
}