using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Models.Account
{
	public class LoginPageModel : ILogins
	{
		[Display(Name = "Přihlašovací jméno")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte zadat přihlašovací jméno")]
		[StringLength(50, MinimumLength = 4, ErrorMessage = "Musíte zadat platné přihlašovací jméno")]
		[PlaceHolder(Text = "Zadejte přihlašovací jméno nebo email")]
		public string UName { get; set; }

		[Display(Name = "Heslo")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Musíte zadat heslo")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Musíte zadat platné heslo")]
		[PlaceHolder(Text = "Zadejte heslo")]
		public string Password { get; set; }

		[Display(Name = "Zapamatovat si mě")]
		public bool RememberMe { get; set; }

		public bool ShowActivationElement { get; set; } = false;
		public bool ShowAccountDisabledElement { get; set; } = false;

		public bool ShowRegistrationErrorElement { get; set; } = false;

		public bool IsValid => !string.IsNullOrWhiteSpace(this.UName) && !string.IsNullOrWhiteSpace(this.Password);

		public bool ShowMessage => !this.ShowActivationElement && !ShowAccountDisabledElement;

		public void ClearPassword()
		{
			this.Password = null;
		}
	}
}