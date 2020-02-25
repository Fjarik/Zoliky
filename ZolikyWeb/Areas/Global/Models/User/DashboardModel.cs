using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZolikyWeb.Areas.Global.Models.User
{
	public class DashboardModel
	{
		public int SchoolID { get; set; }
		public List<DataAccess.Models.School> Schools { get; set; }

		public List<SelectListItem> SchoolSelect => this.Schools
														.Select(x => new SelectListItem {
															Text = x.Name,
															Value = x.ID.ToString(),
														}).Prepend(new SelectListItem {
															Text = "Všechny",
															Value = "0",
														})
														.ToList();

		public DashboardModel() { }

		public DashboardModel(List<DataAccess.Models.School> schools, int schoolId)
		{
			this.Schools = schools;
			this.SchoolID = schoolId;
		}
	}
}