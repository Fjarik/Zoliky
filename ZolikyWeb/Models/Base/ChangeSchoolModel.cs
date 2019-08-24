using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;

namespace ZolikyWeb.Models.Base
{
	public class ChangeSchoolModel
	{
		[Display(Name = "Škola")]
		[Required(ErrorMessage = "Musíte vybrat školu")]
		[Range(1, maximum: int.MaxValue, ErrorMessage = "Musíte vybrat platnou školu")]
		public int SchoolId { get; set; } = 1;

		public IList<School> Schools { get; set; }

		public IEnumerable<SelectListItem> SchoolSelect => this.Schools.Select(x => new SelectListItem() {
																   Value = (x.ID).ToString(),
																   Text = x.Name,
																   Disabled = false,
																   Selected = (x.ID) == SchoolId
															   });

		public ChangeSchoolModel()
		{
			this.Schools = new List<School>();
		}

		public ChangeSchoolModel(int schoolId) : this()
		{
			this.SchoolId = schoolId;
		}
	}
}