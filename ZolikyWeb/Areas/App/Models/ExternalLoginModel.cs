using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.App.Models
{
	public class ExternalLoginModel
	{
		public string Provider { get; set; }
		public string Key { get; set; }
		public bool Add { get; set; }

		public string TargetAction => this.Add ? "AddLogin" : "RemoveLogin";
		public bool IsChecked => !this.Add;
		public string CheckId => $"check{this.Provider}";

		public ExternalLoginModel() { }

		public ExternalLoginModel(string provider, bool add, string key = "")
		{
			Provider = provider;
			this.Add = add;
			Key = key;
		}
	}
}