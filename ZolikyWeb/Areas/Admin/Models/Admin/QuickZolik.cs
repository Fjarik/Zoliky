using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
		public ZolikType Type { get; set; }

		[Required(ErrorMessage = "Musíte vybrat předmět")]
		[Range(1, int.MaxValue)]
		public int SubjectID { get; set; }

#region Lists

		public List<DataAccess.Models.Subject> Subjects { get; set; }

		public IEnumerable<ZolikType> ZolikTypes => Enum.GetValues(typeof(ZolikType))
														.Cast<ZolikType>()
														.Where(x => !x.IsTesterType());

		public IEnumerable<SelectListItem> TypeSelect => this.ZolikTypes
															 .Select(x => new SelectListItem {
																 Value = ((int) x).ToString(),
																 Text = x.GetDescription(),
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

		public QuickZolik(List<DataAccess.Models.Subject> subjects)
		{
			this.Subjects = subjects;
		}
	}
}