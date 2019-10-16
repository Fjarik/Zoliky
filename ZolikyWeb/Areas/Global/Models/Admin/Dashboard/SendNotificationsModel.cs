using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;

namespace ZolikyWeb.Areas.Global.Models.Admin.Dashboard
{
	public class SendNotificationsModel : IValidable
	{
		[Display(Name = "Nadpis")]
		[Required(ErrorMessage = "Musíte zadat nadpis upozornění")]
		[StringLength(50)]
		public string Title { get; set; }

		[Display(Name = "Obsah")]
		[Required(ErrorMessage = "Musíte zadat obsah notifikace")]
		[StringLength(200)]
		public string Subtitle { get; set; }

		[Display(Name = "ID uživatele")]
		[Required(ErrorMessage = "Musíte zadat ID uživatele")]
		[Range(1, int.MaxValue, ErrorMessage = "Musíte zadat platné ID")]
		public int ToID { get; set; }

		public bool IsValid => !Methods.AreNullOrWhiteSpace(this.Title, this.Subtitle);
	}
}