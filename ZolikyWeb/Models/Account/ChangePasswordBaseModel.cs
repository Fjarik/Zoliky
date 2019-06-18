using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Models.Account
{
	public class ChangePasswordBaseModel : IPasswordable
	{
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
		[Compare(nameof(Password), ErrorMessage = "Hesla se musí shodovat")]
		public string RepeatPassword { get; set; }

		public virtual bool IsValid => (!string.IsNullOrWhiteSpace(Password) &&
										!string.IsNullOrWhiteSpace(RepeatPassword) &&
										this.Password == this.RepeatPassword);

		public virtual void ClearPasswords()
		{
			this.Password = "";
			this.RepeatPassword = "";
		}
	}
}