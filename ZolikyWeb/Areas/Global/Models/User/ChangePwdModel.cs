using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Models.User
{
	public class ChangePwdModel
	{
		public int UserID { get; set; }

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
		public string PasswordRepeat { get; set; }

		public bool IsValid => this.UserID > 0 &&
							   !string.IsNullOrWhiteSpace(Password) &&
							   !string.IsNullOrWhiteSpace(PasswordRepeat) &&
							   Password == PasswordRepeat;

		public ChangePwdModel() { }

		public ChangePwdModel(int userID)
		{
			UserID = userID;
		}
	}
}