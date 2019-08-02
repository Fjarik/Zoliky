using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using SharedLibrary.Interfaces;

namespace ZolikyWeb.Areas.Admin.Models.School
{
	public sealed class SchoolModel : DataAccess.Models.School, IValidable
	{
		public bool AllowEdit { get; set; }

		public IEnumerable<SelectListItem> ClassList => this.Classes.Select(x=> new SelectListItem {
			Text = $"{x.Name} ({x.Since.Year} - {x.Graduation.Year})",
			Value = x.ID.ToString(),
			Disabled = !x.Enabled
		});

		public IEnumerable<SelectListItem> SubjectList => this.Subjects.Select(x => new SelectListItem {
			Text = x.Name,
			Value = x.ID.ToString()
		});

		public bool IsValid => true;

		public SchoolModel() { }

		public SchoolModel(DataAccess.Models.School s, bool allowEdit)
		{
			this.AllowEdit = allowEdit;

			this.ID = s.ID;
			this.Type = s.Type;
			this.Name = s.Name;
			this.Street = s.Street;
			this.City = s.City;
			this.AllowTransfer = s.AllowTransfer;
			this.AllowTeacherRemove = s.AllowTeacherRemove;
			this.AllowZolikSplik = s.AllowZolikSplik;
			this.Classes = s.Classes;
			this.Subjects = s.Subjects;
			this.Users = new List<DataAccess.Models.User>();

		}

	}
}