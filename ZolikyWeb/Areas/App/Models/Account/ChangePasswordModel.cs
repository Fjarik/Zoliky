using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;
using ZolikyWeb.Models.Account;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Models.Account
{
	public class ChangePasswordModel : ChangePasswordBaseModel, IChangePassword
	{
		[Display(Name = "Aktuální heslo")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Musíte vyplnit aktuální heslo")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Heslo musí být dlouhé minimálně {2} znaků")]
		public string OldPassword { get; set; }

		public string CurrentUserGuid { get; set; }

		public ChangePasswordModel() : this("") { }

		public ChangePasswordModel(string guid)
		{
			this.CurrentUserGuid = guid;
		}

		public override bool IsValid => (base.IsValid &&
										 !string.IsNullOrWhiteSpace(OldPassword));

		public override void ClearPasswords()
		{
			base.ClearPasswords();
			OldPassword = "";
		}
	}
}