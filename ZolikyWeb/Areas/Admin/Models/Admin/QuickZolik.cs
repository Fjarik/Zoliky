using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Shared;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Models.Admin
{
	public class QuickZolik
	{
		[Required(ErrorMessage = "Musíte zadat, za co byl žolík udělen")]
		[PlaceHolder(Text = "Zadejte, za co byl žolík udělen")]
		[StringLength(200)]
		public string Title { get; set; }

		[Required(ErrorMessage = "Musíte vybrat typ")]
		public int TypeID { get; set; }

		[Required(ErrorMessage = "Musíte vybrat předmět")]
		[Range(1, int.MaxValue)]
		public int SubjectID { get; set; }

#region Lists

		public List<DataAccess.Models.Subject> Subjects { get; set; }
		public List<DataAccess.Models.ZolikType> Types { get; set; }

		public IEnumerable<ZolikType> ZolikTypes => Types.Where(x => !x.IsTestType);

		public IEnumerable<SelectListItem> TypeSelect => this.ZolikTypes
															 .Select(x => new SelectListItem {
																 Value = (x.ID).ToString(),
																 Text = x.FriendlyName,
																 Disabled = false
															 });

		public IEnumerable<SelectListItem> SubjectSelect => this.Subjects
																.Select(x => new SelectListItem {
																	Value = x.ID.ToString(),
																	Text = x.Name,
																	Disabled = false
																}).Prepend(new SelectListItem {
																	Value = "-1",
																	Text = "Vyberte předmět",
																	Disabled = true,
																	Selected = this.SubjectID == -1
																});

#endregion

		public QuickZolik() { }

		public QuickZolik(List<DataAccess.Models.ZolikType> types, List<DataAccess.Models.Subject> subjects)
		{
			this.Types = types;
			this.Subjects = subjects;
		}
	}
}