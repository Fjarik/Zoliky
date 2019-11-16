using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SharedLibrary.Shared;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Models.Admin.SettingsModels
{
	public class ProjectSettingsModel
	{
		public bool RegistrationEnabled { get; set; }

		[Display(Name = "Speciální den - název")]
		[Required(ErrorMessage = "Musíte zadat název speciálního dne")]
		[PlaceHolder(Text = "Zadejte název speciálního dne")]
		[StringLength(100)]
		public string SpecialText { get; set; }

		[Display(Name = "Speciální den - datum")]
		[Required(ErrorMessage = "Musíte zadat datum speciálního dne")]
		[PlaceHolder(Text = "Zadejte datum speciálního dne")]
		public DateTime SpecialDate { get; set; }

		public IDictionary<string, object> Dictionary => new ConcurrentDictionary<string, object>() {
			[ProjectSettingKeys.RegistrationEnabled] = this.RegistrationEnabled,
			[ProjectSettingKeys.SpecialText] = this.SpecialText,
			[ProjectSettingKeys.SpecialDate] = this.SpecialDate
		};
	}
}