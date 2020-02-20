using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared.ApiModels;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Models.Zoliky
{
	public sealed class ZolikLockModel : ZolikLock
	{
		[Required(ErrorMessage = "Musíte zadat na kterou známku chcete použít žolíka")]
		[StringLength(maximumLength: 50,
			MinimumLength = 2,
			ErrorMessage = "Text musí být dlouhý minimálně 2 znaky a maximálně 50 znaků")]
		[PlaceHolder(Text = "Zadejte text")]
		[Display(Name = "Použít na:")]
		public override string Lock { get; set; }

		public string Title { get; set; }
		public string Type { get; set; }

		public ZolikLockModel() { }

		public ZolikLockModel(Zolik zolik)
		{
			this.ZolikId = zolik.ID;
			this.Title = zolik.Title;
			this.Type = zolik.Type.FriendlyName;
		}
	}
}