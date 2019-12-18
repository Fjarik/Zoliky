using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;

namespace ZolikyWeb.Areas.Admin.Models.Zoliky
{
	public class ZolikDashboardModel
	{
		public bool OnlyEnabled { get; set; }
		public int ClassID { get; set; }
		public IEnumerable<DataAccess.Models.Zolik> Zoliks { get; set; }
		public IEnumerable<DataAccess.Models.Class> Classes { get; set; }

		public List<SelectListItem> ClassesDrop => this.Classes.Select(x => new SelectListItem {
			Disabled = !x.Enabled,
			Text = x.Name,
			Value = x.ID.ToString(),
			Selected = this.ClassID == ClassID,
		}).Prepend(new SelectListItem {
			Disabled = false,
			Text = "Všechny",
			Value = 0.ToString(),
			Selected = this.ClassID == ClassID,
		}).ToList();

		public ZolikDashboardModel()
		{
			this.Zoliks = new List<DataAccess.Models.Zolik>();
			this.Classes = new List<DataAccess.Models.Class>();
		}

		public ZolikDashboardModel(IEnumerable<Zolik> zoliks, IEnumerable<DataAccess.Models.Class> classes,
								   bool onlyEnabled, int? classId = null) : this()
		{
			this.Zoliks = zoliks;
			if (classId != null && classId != 0) {
				this.Zoliks = this.Zoliks.Where(x => x.OwnerClassId == classId);
			}
			this.Classes = classes;
			this.OnlyEnabled = onlyEnabled;
			if (classId == null) {
				this.ClassID = 0;
			} else {
				this.ClassID = (int) classId;
			}
		}
	}
}