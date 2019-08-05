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
	public class ClassModel : UniversalModel<DataAccess.Models.Class>, IClass
	{
		public override bool AllowRemove { get; set; }
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; }
		public override int PreviousID { get; set; }
		public override int ID { get; set; }
		public override int NextID { get; set; }
		public override string ActionName { get; set; }

		public List<DataAccess.Models.School> Schools { get; set; }

		public IEnumerable<SelectListItem> SchoolList => this.Schools
															 .OrderBy(x => x.Name)
															 .Select(x => new SelectListItem {
																 Text = $"{x.Name}, {x.Street}, {x.City}",
																 Value = x.ID.ToString(),
																 Disabled = false
															 });

		public List<string> StudentNames { get; set; }

		public IEnumerable<SelectListItem> StudentList => this.StudentNames
															  .OrderBy(x => x)
															  .Select(x => new SelectListItem {
																  Text = x,
																  Value = "0"
															  });

		public override bool IsValid => (this.ID == -1 || this.ID > 0) &&
										SchoolID > 0 &&
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

		public bool Enabled { get; set; }

		public string SchoolName { get; set; }

#endregion

		public ClassModel() : base()
		{
			this.Schools = new List<DataAccess.Models.School>();
			this.StudentNames = new List<string>();
		}

		public static ClassModel CreateModel(List<DataAccess.Models.School> allSchools)
		{
			return new ClassModel() {
				ID = -1,
				ActionName = "Create",
				AllowEdit = true,
				IsCreate = true,
				Schools = allSchools
			};
		}

		public ClassModel(DataAccess.Models.Class ent,
						  List<DataAccess.Models.School> allSchools,
						  List<string> studentNames,
						  bool allowEdit,
						  int previousId,
						  int nextId) : base(ent, allowEdit, previousId, nextId)
		{
			this.Schools = allSchools;
			this.StudentNames = studentNames;
			this.SchoolID = ent.SchoolID;
			this.Name = ent.Name;
			this.Since = ent.Since;
			this.Graduation = ent.Graduation;
			this.Enabled = ent.Enabled;
			this.SchoolName = ent.School.Name;
		}
	}
}