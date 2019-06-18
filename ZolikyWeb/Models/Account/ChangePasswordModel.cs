using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Models.Account
{
	public class ChangePasswordModel : ChangePasswordBaseModel, IChangePasswordCode
	{
		public string Code { get; set; }

		public ChangePasswordModel() { }

		public override bool IsValid => (base.IsValid &&
										 !string.IsNullOrWhiteSpace(Code));
	}
}