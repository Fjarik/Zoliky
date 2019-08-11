using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Models.Zoliky
{
	public class ZolikRemoveModel
	{
		public int ID { get; set; }
		public int OwnerID { get; set; }

		public string Title { get; set; }

		public string OwnerName { get; set; }

		[Display(Name = "Důvod odstranění žolíka")]
		[Required(ErrorMessage = "Musíte zadat důvod odstranění žolíka")]
		[PlaceHolder(Text = "Zadejte důvod odstranění žolíka")]
		[StringLength(200)]
		public string Reason { get; set; }
	}
}