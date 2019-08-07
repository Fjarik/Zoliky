using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedLibrary.Interfaces;

namespace ZolikyWeb.Areas.Admin.Models.Subject
{
	public class SubjectEditModel : IValidable
	{
#region 1. Step

		[Display(Name = "Předmět")]
		[Required(ErrorMessage = "Musíte vybrat předmět")]
		[Range(0, maximum: int.MaxValue, ErrorMessage = "Musíte vybrat platný předmět")]
		public int SubjectID { get; set; }

		public IList<DataAccess.Models.Subject> Subjects { get; set; }

		public IEnumerable<SelectListItem> SubjectSelect => this.Subjects
																.Select(x => new SelectListItem {
																	Value = x.ID.ToString(),
																	Text = x.Name,
																	Disabled = false,
																}).Prepend(new SelectListItem {
																	Value = "-1",
																	Text = "Vyberte předmět",
																	Disabled = true,
																	Selected = this.SubjectID == -1
																});

#endregion

#region 2. Step

		[Display(Name = "Vyučující")]
		[Required(ErrorMessage = "Musíte vybrat vyučující")]
		[Range(0, maximum: int.MaxValue, ErrorMessage = "Musíte vybrat platného/platnou vyučující")]
		public int TeacherID { get; set; }

		public IList<IStudent> Teachers { get; set; }

		public IEnumerable<SelectListItem> TeacherSelect => this.Teachers
																.Select(x => new SelectListItem {
																	Value = x.ID.ToString(),
																	Text = x.FullName,
																	Disabled = false
																}).Prepend(new SelectListItem {
																	Value = "-1",
																	Text = "Vyberte učitele",
																	Disabled = true,
																	Selected = this.TeacherID == -1
																});

#endregion

#region 3. Step

		public List<int> ClassIDs { get; set; }

		public IList<IClass> Classes { get; set; }

		public IEnumerable<SelectListItem> ClassSelect => this.Classes
															  .Select(x => new SelectListItem {
																  Value = x.ID.ToString(),
																  Text = x.Name,
																  Disabled = !x.Enabled
															  });

#endregion

		public SubjectEditModel()
		{
			this.Subjects = new List<DataAccess.Models.Subject>();
			this.Teachers = new List<IStudent>();
			this.Classes = new List<IClass>();

			this.SubjectID = -1;
			this.TeacherID = -1;
		}

		public SubjectEditModel(List<DataAccess.Models.Subject> subjects,
								List<IStudent> teachers,
								List<IClass> classes) : this()
		{
			this.Subjects = subjects;
			this.Teachers = teachers;
			this.Classes = classes;
		}

		public bool IsValid => this.SubjectID > 0 &&
							   this.TeacherID > 0;
	}
}