using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared.ApiModels;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Models.Zoliky
{
	public class ZolikTransferModel
	{
		[Display(Name = "Příjemce:")]
		[Required(ErrorMessage = "Musíte vybrat příjemce")]
		[Range(0, int.MaxValue)]
		public int ToID { get; set; }

		[Display(Name = "Žolík")]
		[Required(ErrorMessage = "Musíte vybrat žolíka")]
		[Range(0, int.MaxValue)]
		public int ZolikID { get; set; }

		[Display(Name = "Zpráva")]
		[DataType(DataType.MultilineText)]
		[Required(ErrorMessage = "Musíte vyplnit zprávu")]
		[StringLength(500, MinimumLength = 6, ErrorMessage = "Zpráva musí být dlouhá minimálně {2} znaků")]
		public string Message { get; set; }

		public IList<Zolik> Zoliks { get; set; }

		public bool IsValid => (ToID > 0 && ZolikID > 0 && !string.IsNullOrWhiteSpace(this.Message));

		public IList<SelectListItem> ZolikSelect => this.Zoliks.Select(x => new SelectListItem() {
			Value = x.ID.ToString(),
			Text = $@"{(x.IsLocked ? "(zamčený) " : "")}{x.Title} ({x.Type.GetDescription()})",
			Disabled = !x.CanBeTransfered,
			Selected = x.ID == ZolikID
		}).ToList();

		public IList<SelectListItem> StudentSelect => new List<SelectListItem>();

		public string ZolikyJson => JsonConvert.SerializeObject(this.Zoliks);

		public ZolikTransferModel()
		{
			this.Zoliks = new List<Zolik>();
		}
	}
}