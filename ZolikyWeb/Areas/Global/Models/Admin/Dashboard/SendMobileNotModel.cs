using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Global.Models.Admin.Dashboard
{
	public class SendMobileNotModel
	{
		[Display(Name = "ID příjemce")]
		[Required(ErrorMessage = "Musíte zadat ID příjemce")]
		[Range(1, int.MaxValue, ErrorMessage = "Musíte zadat platné ID")]
		public int ToId { get; set; }

		[Display(Name = "ID žolíka")]
		[Required(ErrorMessage = "Musíte zadat ID žolíka")]
		[Range(1, int.MaxValue, ErrorMessage = "Musíte zadat platné ID")]
		public int ZolikId { get; set; }
	}
}