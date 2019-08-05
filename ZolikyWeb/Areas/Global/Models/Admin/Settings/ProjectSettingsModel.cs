using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedLibrary.Shared;

namespace ZolikyWeb.Areas.Global.Models.Admin.SettingsModels
{
	public class ProjectSettingsModel
	{
		public bool RegistrationEnabled { get; set; }

		public IDictionary<string, bool> Dictionary => new ConcurrentDictionary<string, bool>() {
			[ProjectSettingKeys.RegistrationEnabled] = this.RegistrationEnabled
		};
	}
}