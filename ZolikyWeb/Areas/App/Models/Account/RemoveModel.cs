using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Models.Account
{
	public class RemoveModel
	{
		[Display(Name = "Heslo")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Musíte zadat heslo")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Musíte zadat platné heslo")]
		[PlaceHolder(Text = "Zadejte heslo")]
		public string Password { get; set; }
	}
}