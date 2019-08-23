using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using Microsoft.Ajax.Utilities;
using SharedLibrary.Enums;
using SharedLibrary.Shared;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Models.Account
{
	public class RegisterModel
	{
#region 1. step

		[Display(Name = "Email")]
		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessage = "Musíte vyplnit email")]
		[PlaceHolder(Text = "Zadejte email")]
		[StringLength(300, MinimumLength = 6, ErrorMessage = "Email musí být dlouhý minimálně {2} znaků")]
		public string Email { get; set; }

		[Display(Name = "Heslo")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Musíte vyplnit heslo")]
		[PlaceHolder(Text = "Zadejte heslo")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Heslo musí být dlouhé minimálně {2} znaků")]
		public string Password { get; set; }

		[Display(Name = "Heslo znovu")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Musíte vyplnit znovu heslo")]
		[PlaceHolder(Text = "Zadejte heslo znovu")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Heslo musí být dlouhé minimálně 6 znaků")]
		//[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Hesla se musí shodovat")]
		public string PasswordRepeat { get; set; }

#endregion

#region 2. step

		[Display(Name = "Křestní jméno")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte vyplnit křestní jméno")]
		[PlaceHolder(Text = "Zadejte křestní jméno")]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "Křestní jméno musí být dlouhé minimálně {2} znaky")]
		[RegularExpression(Ext.NameRegEx, ErrorMessage = "Jméno není ve správném formátu")]
		public string Firstname { get; set; }

		[Display(Name = "Příjmení")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte vyplnit příjmení")]
		[PlaceHolder(Text = "Zadejte příjmení")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Příjmení musí být dlouhé minimálně {2} znaky")]
		[RegularExpression(Ext.NameRegEx, ErrorMessage = "Příjmení není ve správném formátu")]
		public string Lastname { get; set; }

		[Display(Name = "Pohlaví")]
		[Required(ErrorMessage = "Musíte vybrat své pohlaví")]
		[Range(0, maximum: 9, ErrorMessage = "Musíte vybrat platné pohlaví")]
		public int Gender { get; set; } = -1;

		[Display(Name = "Třída")]
		[Required(ErrorMessage = "Musíte vybrat třídu")]
		[Range(0, maximum: int.MaxValue, ErrorMessage = "Musíte vybrat platnou třídu")]
		public int ClassId { get; set; } = -1;

		[Display(Name = "Škola")]
		[Required(ErrorMessage = "Musíte vybrat školu")]
		[Range(1, maximum: int.MaxValue, ErrorMessage = "Musíte vybrat platnou školu")]
		public int SchoolId { get; set; } = -1;

#endregion

#region 3. step

		[Display(Name = "Uživatelské jméno")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte vyplnit uživatelské jméno")]
		[PlaceHolder(Text = "Zadejte uživatelské jméno")]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "Uživatelské jméno musí být dlouhé minimálně {2} znaky")]
		[RegularExpression(Ext.UsernameRegEx, ErrorMessage = "Přezdívka může obsahovat pouze znaky a-Z a čísla")]
		public string Username { get; set; }

		[Display(Name = "Souhlasím se smluvními podmínkami Žolíků*")]
		[Required(ErrorMessage = "Musíte souhlasit s podmínkami")]
		[Range(typeof(bool), "true", "true", ErrorMessage = "Musíte souhlasit s podmínkami")]
		public bool TOS { get; set; }

		[Display(Name = "Souhlasím se zpracováním osobních údajů*")]
		[Required(ErrorMessage = "Musíte souhlasit se zpracováním osobních údajů")]
		[Range(typeof(bool), "true", "true", ErrorMessage = "Musíte souhlasit se zpracováním osobních údajů")]
		public bool PP { get; set; }

		[Display(Name = "Přihlásit se k odběru novinek")]
		public bool Newsletter { get; set; }

		[Display(Name = "Mám zájem o budoucí nabídky")]
		public bool FutureNews { get; set; }

#endregion

#region Lists

		public IEnumerable<Sex> Genders = Enum.GetValues(typeof(Sex)).Cast<Sex>().SkipLast(1);

		public IList<Class> Classes { get; set; } = new List<Class>();

		public IList<School> Schools { get; set; }

		public IEnumerable<SelectListItem> GenderSelect => this.Genders.Select(x => new SelectListItem() {
																   Value = ((int) x).ToString(),
																   Text = x.GetDescription(),
																   Disabled = false,
																   Selected = ((int) x) == Gender
															   })
															   .Prepend(new SelectListItem() {
																   Value = "-1",
																   Text = "Vyberte pohlaví",
																   Disabled = true,
																   Selected = -1 == Gender
															   });

		public IEnumerable<SelectListItem> ClassSelect => this.Classes.Select(x => new SelectListItem() {
																  Value = x.ID.ToString(),
																  Text = x.Name,
																  Disabled = !x.Enabled,
																  Selected = x.ID == ClassId
															  })
															  .Prepend(new SelectListItem() {
																  Value = "0",
																  Text = "Veřejnost",
																  Disabled = false,
																  Selected = 0 == ClassId
															  })
															  .Prepend(new SelectListItem() {
																  Value = "-1",
																  Text = "Vyberte třídu",
																  Disabled = true,
																  Selected = -1 == ClassId
															  });

		public IEnumerable<SelectListItem> SchoolSelect => this.Schools.Select(x => new SelectListItem() {
																   Value = (x.ID).ToString(),
																   Text = x.Name,
																   Disabled = false,
																   Selected = (x.ID) == SchoolId
															   })
															   .Prepend(new SelectListItem() {
																   Value = "-1",
																   Text = "Vyberte školu",
																   Disabled = true,
																   Selected = -1 == SchoolId
															   });

#endregion

#region Other

		public bool IsValid => !string.IsNullOrWhiteSpace(Email) &&
							   Methods.IsEmailValid(Email) &&
							   !string.IsNullOrWhiteSpace(Password) &&
							   !string.IsNullOrWhiteSpace(PasswordRepeat) &&
							   Password == PasswordRepeat &&
							   !string.IsNullOrWhiteSpace(Firstname) &&
							   Regex.Match(Firstname, Ext.NameRegEx).Success &&
							   !string.IsNullOrWhiteSpace(Lastname) &&
							   Regex.Match(Lastname, Ext.NameRegEx).Success &&
							   !string.IsNullOrWhiteSpace(Username) &&
							   Regex.Match(Username, Ext.UsernameRegEx).Success &&
							   Gender >= 0 &&
							   Enum.IsDefined(typeof(Sex), (byte) Gender) &&
							   ClassId >= 0 &&
							   SchoolId > 0 &&
							   TOS &&
							   PP;

		public void ClearPasswords()
		{
			this.Password = null;
			this.PasswordRepeat = null;
		}

#endregion
	}
}