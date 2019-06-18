using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedLibrary.Shared;

namespace ZolikyWeb.Areas.App.Models.Account
{
	public class NotificationModel
	{
		public bool Newsletter { get; set; }
		public bool FutureNews { get; set; }

		public IDictionary<string, bool> Dictionary => new ConcurrentDictionary<string, bool>()
		{
			[SettingKeys.Newsletter] = this.Newsletter,
			[SettingKeys.FutureNews] = this.FutureNews,
		};
	}
}