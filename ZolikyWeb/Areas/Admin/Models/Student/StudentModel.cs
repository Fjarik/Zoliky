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

namespace ZolikyWeb.Areas.Admin.Models.Student
{
	public class StudentModel : UniversalModel<User>
	{
		public override bool AllowRemove { get; set; }
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; }
		public override int ID { get; set; }
		public override string ActionName { get; set; }
		public override bool IsValid { get; }

#region Entity

		[Required(ErrorMessage = "Musíte vybrat třídu")]
		[Range(1, int.MaxValue)]
		public int ClassID { get; set; }

		[Required(ErrorMessage = "Musíte vybrat školu")]
		[Range(1, int.MaxValue)]
		public int SchoolID { get; set; }

		[Display(Name = "Uživatelské jméno")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte vyplnit uživatelské jméno")]
		[PlaceHolder(Text = "Zadejte uživatelské jméno")]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "Uživatelské jméno musí být dlouhé minimálně {2} znaky")]
		[RegularExpression(Ext.UsernameRegEx, ErrorMessage = "Přezdívka může obsahovat pouze znaky a-Z a čísla")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Musíte vyplnit email")]
		[PlaceHolder(Text = "Zadejte email")]
		[StringLength(300, MinimumLength = 6, ErrorMessage = "Email musí být dlouhý minimálně {2} znaků")]
		public string Email { get; set; }

		[Display(Name = "Křestní jméno")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte vyplnit křestní jméno")]
		[PlaceHolder(Text = "Zadejte křestní jméno")]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "Křestní jméno musí být dlouhé minimálně {2} znaky")]
		[RegularExpression(Ext.NameRegEx, ErrorMessage = "Jméo není ve správném formátu")]
		public string Name { get; set; }

		[Display(Name = "Příjmení")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte vyplnit příjmení")]
		[PlaceHolder(Text = "Zadejte příjmení")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Příjmení musí být dlouhé minimálně {2} znaky")]
		[RegularExpression(Ext.NameRegEx, ErrorMessage = "Jméo není ve správném formátu")]
		public string Lastname { get; set; }

		[Required(ErrorMessage = "Musíte vybrat pohlaví")]
		public Sex Sex { get; set; }

		[Display(Name = "Ano/Ne")]
		public bool Enabled { get; set; }

		[Display(Name = "Ano/Ne")]
		public bool EmailConfirmed { get; set; }

		public string SchoolName { get; set; }
		public string ClassName { get; set; }

		public string ClassDate { get; set; }

#endregion

#region Lists

		public List<DataAccess.Models.Class> Classes { get; set; }

		public IEnumerable<SelectListItem> ClassSelect => this.Classes.Select(x => new SelectListItem() {
																  Value = x.ID.ToString(),
																  Text = x.Name,
																  Disabled = !x.Enabled,
															  })
															  .Prepend(new SelectListItem() {
																  Value = "-1",
																  Text = "Vyberte třídu",
																  Disabled = true,
																  Selected = -1 == ClassID
															  });

		public List<School> Schools { get; set; }

		public IEnumerable<SelectListItem> SchoolSelect => this.Schools.Select(x => new SelectListItem() {
																   Value = x.ID.ToString(),
																   Text = x.Name,
															   })
															   .Prepend(new SelectListItem() {
																   Value = "-1",
																   Text = "Vyberte školu",
																   Disabled = true,
																   Selected = -1 == SchoolID
															   });

		public IEnumerable<Sex> Genders = Enum.GetValues(typeof(Sex)).Cast<Sex>().SkipLast(1);

		public IEnumerable<SelectListItem> GenderSelect => this.Genders.Select(x => new SelectListItem() {
																   Value = ((int) x).ToString(),
																   Text = x.GetDescription(),
																   Disabled = false,
															   })
															   .Prepend(new SelectListItem() {
																   Value = "-1",
																   Text = "Vyberte pohlaví",
																   Disabled = true,
																   Selected = -1 == (int) this.Sex
															   });

#endregion

		public StudentModel() : base()
		{
			this.AllowRemove = false;
		}

		public StudentModel(User ent,
							List<DataAccess.Models.Class> classes,
							List<School> schools,
							bool allowEdit,
							int previousId,
							int nextId,
							string url) : base(ent, allowEdit, previousId, nextId, url)
		{
			this.Classes = classes;
			this.Schools = schools;

			this.ClassID = ent.ClassID ?? -1;
			this.SchoolID = ent.SchoolID;
			this.Username = ent.Username;
			this.Email = ent.Email;
			this.Name = ent.Name;
			this.Lastname = ent.Lastname;
			this.Sex = ent.Sex;
			this.Enabled = ent.Enabled;
			this.EmailConfirmed = ent.EmailConfirmed;
			this.SchoolName = ent.SchoolName;
			this.ClassName = ent.ClassName;
			this.ClassDate = $"{ent.Class.Since.Year} - {ent.Class.Graduation.Year}";

			this.AllowRemove = false;
		}
	}
}