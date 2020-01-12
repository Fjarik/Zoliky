using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedLibrary.Interfaces;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Models.Class
{
	public sealed class ClassModel : UniversalModel<DataAccess.Models.Class>, IClass
	{
		public override bool AllowRemove { get; set; }
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; }
		public override int ID { get; set; }
		public override string ActionName { get; set; }


		public List<string> StudentNames { get; set; }

		public IEnumerable<SelectListItem> StudentList => this.StudentNames
															  .OrderBy(x => x)
															  .Select(x => new SelectListItem {
																  Text = x,
																  Value = "0"
															  });

		public override bool IsValid => (this.ID == -1 || this.ID > 0) &&
										!string.IsNullOrWhiteSpace(this.Name);

#region Entity

		[Display(Name = "Škola")]
		[Required(ErrorMessage = "Musíte vybrat školu")]
		[Range(1, int.MaxValue)]
		public int SchoolID { get; set; }

		[Display(Name = "Název")]
		[Required(ErrorMessage = "Musíte zadat název třídy")]
		[PlaceHolder(Text = "Zadejte název třídy")]
		[StringLength(10)]
		public string Name { get; set; }

		[Display(Name = "Datum nástupu")]
		[Required(ErrorMessage = "Musíte zadat datum nástupu")]
		[PlaceHolder(Text = "Zadejte datum nástupu")]
		[DataType(DataType.Date)]
		public DateTime Since { get; set; }

		[Display(Name = "Ukončení studia")]
		[Required(ErrorMessage = "Musíte zadat datum ukončení studia")]
		[PlaceHolder(Text = "Zadejte datum ukončení studia")]
		[DataType(DataType.Date)]
		public DateTime Graduation { get; set; }

		[Display(Name = "Barva")]
		[Required(ErrorMessage = "Musíte vybrat barvu")]
		[DataType(DataType.Text)]
		public string Colour { get; set; }

		public bool Enabled { get; set; }

		public string SchoolName { get; set; }

#endregion

		public ClassModel() : base()
		{
			this.StudentNames = new List<string>();
			this.AllowRemove = false;
		}

		public static ClassModel CreateModel()
		{
			var currentYear = DateTime.Today.Year;
			return new ClassModel() {
				ID = -1,
				ActionName = "Create",
				AllowEdit = true,
				IsCreate = true,
				Since = new DateTime(currentYear, 9, 1),
				Graduation = new DateTime(currentYear + 4, 5, 31)
			};
		}

		public ClassModel(DataAccess.Models.Class ent,
						  List<string> studentNames,
						  bool allowEdit,
						  int previousId,
						  int nextId) : base(ent, allowEdit, previousId, nextId)
		{
			this.StudentNames = studentNames;
			this.SchoolID = ent.SchoolID;
			this.Name = ent.Name;
			this.Since = ent.Since;
			this.Graduation = ent.Graduation;
			this.Enabled = ent.Enabled;
			this.Colour = ent.Colour;
			this.SchoolName = ent.School.Name;
			this.AllowRemove = !this.IsCreate &&
							   !studentNames.Any();
		}
	}
}