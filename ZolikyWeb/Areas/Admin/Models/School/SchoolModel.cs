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
		public int PreviousID { get; set; }
		public int NextID { get; set; }

		public List<int> SubjectIds { get; set; }
		public List<Subject> AllSubjects { get; set; } = new List<Subject>();

		public bool IsFirst => this.PreviousID == 0;
		public bool IsLast => this.NextID == 0;

		public IEnumerable<SelectListItem> ClassList => this.Classes
															.OrderBy(x => x.Name)
															.ThenByDescending(x => x.Since.Year)
															.Select(x => new SelectListItem {
																Text =
																	$"{x.Name} ({x.Since.Year} - {x.Graduation.Year})",
																Value = x.ID.ToString(),
																Disabled = !x.Enabled
															});

		public IEnumerable<SelectListItem> SubjectList => this.AllSubjects
															  .OrderBy(x => x.Name)
															  .Select(x => new SelectListItem {
																  Text = x.Name,
																  Value = x.ID.ToString(),
																 // Selected = this.Subjects.Any(y => y.ID == x.ID)
															  });

		public bool IsValid => true;

		public SchoolModel()
		{
			this.SubjectIds = new List<int>();
		}

		public SchoolModel(DataAccess.Models.School s, List<Subject> allSubjects, bool allowEdit, int previousId,
						   int nextId) : this()
		{
			this.AllSubjects = allSubjects;
			this.AllowEdit = allowEdit;
			this.PreviousID = previousId;
			this.NextID = nextId;

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
			this.SubjectIds = s.Subjects.Select(x => x.ID).ToList();
		}
	}
}