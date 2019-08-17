using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Models.Home
{
	public class HomeModel
	{
		public int StudentCount { get; set; }
		public int SchoolCount { get; set; }
		public int ZolikCount { get; set; }
		public int AchievementCount { get; set; }

		public ContactUsModel ContactUs { get; set; }

		public HomeModel()
		{
			this.ContactUs = new ContactUsModel();
		}
	}
}