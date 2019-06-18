using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using ZolikyWeb.Models.Account;

namespace ZolikyWeb.Areas.App.Models.Account
{
	public class GeneralChangeModel : EmailModel, IValidable
	{
		[Display(Name = "Uživatelské jméno")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte vyplnit uživatelské jméno")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Uživatelské jméno musí být dlouhé minimálně {2} znaků")]
		public string Username { get; set; }

		[Display(Name = "Pohlaví")]
		[Required(ErrorMessage = "Musíte vybrat své pohlaví")]
		[Range(0, maximum: 9, ErrorMessage = "Musíte vybrat platné pohlaví")]
		public int Gender { get; set; }

#region Lists

		public IEnumerable<Sex> Genders = Enum.GetValues(typeof(Sex)).Cast<Sex>().SkipLast(1);


		public IEnumerable<SelectListItem> GenderSelect => this.Genders.Select(x => new SelectListItem()
		{
			Value = ((int) x).ToString(),
			Text = x.GetDescription(),
			Disabled = false,
			Selected = ((int) x) == Gender
		});

#endregion

#region Other

		public override bool IsValid => base.IsValid &&
		                                !Methods.AreNullOrWhiteSpace(this.Username) &&
		                                Gender >= 0 &&
		                                Enum.IsDefined(typeof(Sex), (byte) Gender);

		public GeneralChangeModel()
		{
		}

		public GeneralChangeModel(string username, string email, int gender)
		{
			this.Username = username;
			this.Email = email;
			this.Gender = gender;
		}

#endregion
	}
}