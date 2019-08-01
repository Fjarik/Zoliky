using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models.Admin.Dashboard
{
	public class SendMobileNotModel
	{
		[Display(Name = "ID příjemce")]
		[Required(ErrorMessage = "Musíte zadat ID příjemce")]
		[Range(0, int.MaxValue)]
		public int ToId { get; set; }

		[Display(Name = "ID žolíka")]
		[Required(ErrorMessage = "Musíte zadat ID žolíka")]
		[Range(0, int.MaxValue)]
		public int ZolikId { get; set; }
	}
}