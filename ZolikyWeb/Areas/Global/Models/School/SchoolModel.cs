using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Models.School
{
	public sealed class SchoolModel : UniversalModel<DataAccess.Models.School>
	{
		public override bool AllowRemove { get; set; } = false;
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; } = false;
		public override int ID { get; set; }
		public override string ActionName { get; set; }

#region Entity

		[Display(Name = "Název")]
		[Required(ErrorMessage = "Musíte zadat název školy")]
		[PlaceHolder(Text = "Zadejte název školy")]
		[StringLength(200)]
		public string Name { get; set; }

		public SharedLibrary.Enums.SchoolTypes Type { get; set; }

		[Display(Name = "Ulice")]
		[Required(ErrorMessage = "Musíte zadat ulici")]
		[PlaceHolder(Text = "Zadejte ulici")]
		[StringLength(200)]
		public string Street { get; set; }

		[Display(Name = "Město")]
		[Required(ErrorMessage = "Musíte zadat město")]
		[PlaceHolder(Text = "Zadejte město")]
		[StringLength(200)]
		public string City { get; set; }

		[Display(Name = "Ano/Ne")]
		public bool AllowTransfer { get; set; }

		[Display(Name = "Povoleno/Zakázáno")]
		public bool AllowTeacherRemove { get; set; }

		[Display(Name = "Povoleno/Zakázáno")]
		public bool AllowZolikSplik { get; set; }

		public ICollection<Class> Classes { get; set; }
		public ICollection<DataAccess.Models.User> Users { get; set; }

		[Display(Name = "Druh")]
		[Required(ErrorMessage = "Musíte vybrat druh školy")]
		[Range(0, 3)]
		public byte TypeID { get; set; }

#endregion

#region Extends

		public List<int> SubjectIds { get; set; }
		public List<DataAccess.Models.Subject> AllSubjects { get; set; }

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
																  Disabled = x.ID == 4
																  // Selected = this.Subjects.Any(y => y.ID == x.ID)
															  });

		public IEnumerable<SchoolTypes> Types => Enum.GetValues(typeof(SchoolTypes)).Cast<SchoolTypes>();

		public IEnumerable<SelectListItem> TypeSelect => this.Types
															 .Select(x => new SelectListItem {
																 Value = ((byte) x).ToString(),
																 Text = x.GetDescription(),
																 Disabled = false,
																 Selected = x == Type
															 });

#endregion

		public override bool IsValid => (this.ID == -1 || this.ID > 0) &&
										Enum.IsDefined(typeof(SchoolTypes), this.TypeID) &&
										!Methods.AreNullOrWhiteSpace(this.Name, this.Street, this.City);

		public SchoolModel() : base()
		{
			this.SubjectIds = new List<int>() {
				4
			};
			this.Classes = new List<Class>();
			this.AllSubjects = new List<DataAccess.Models.Subject>();
			this.Users = new List<DataAccess.Models.User>();
			this.AllowRemove = false;
		}

		public static SchoolModel CreateModel(List<DataAccess.Models.Subject> allSubjects)
		{
			return new SchoolModel() {
				ID = -1,
				AllSubjects = allSubjects,
				ActionName = "Create",
				AllowEdit = true,
				IsCreate = true
			};
		}

		public SchoolModel(DataAccess.Models.School s,
						   List<DataAccess.Models.Subject> allSubjects,
						   bool allowEdit,
						   int previousId,
						   int nextId) : base(s, allowEdit, previousId, nextId)
		{
			this.AllSubjects = allowEdit ? allSubjects : s.Subjects;

			this.ID = s.ID;
			this.Type = s.Type;
			this.TypeID = (byte) s.Type;
			this.Name = s.Name;
			this.Street = s.Street;
			this.City = s.City;
			this.AllowTransfer = s.AllowTransfer;
			this.AllowTeacherRemove = s.AllowTeacherRemove;
			this.AllowZolikSplik = s.AllowZolikSplik;
			this.Classes = s.Classes;
			//this.Subjects = s.Subjects;
			if (allowEdit) {
				this.SubjectIds = s.SchoolSubjects.Select(x => x.SubjectID).ToList();
				if (!SubjectIds.Contains(4)) {
					SubjectIds.Add(4);
				}
			}

			this.AllowRemove = !IsCreate &&
							   !s.Classes.Any() &&
							   !s.Users.Any();
		}
	}
}