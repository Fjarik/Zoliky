using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared.ApiModels;

namespace ZolikyWeb.Models.Account
{
	public class StudentsDrop
	{
		public string Uqid { get; set; } = "";
		public int? StudentID { get; set; } = null;
		public IList<IStudent> Students { get; set; }

		public IList<SelectListItem> StudentSelect => this.Students.Select(x => new SelectListItem() {
			Value = x.ID.ToString(),
			Text =
				$@"{x.FullName} ({x.ClassName})",
			Disabled = false,
			Selected = x.ID == StudentID
		}).ToList();

		public StudentsDrop()
		{
			Students = new List<IStudent>();
		}
	}
}