using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess.Models;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Global.Models.Admin.SettingsModels;

namespace ZolikyWeb.Areas.Global.Models.Admin
{
	public class SettingsModel
	{
		public ProjectSettingsModel ProjectSettings { get; set; }

		public SettingsModel(ICollection<ProjectSetting> settings)
		{
			this.ProjectSettings = new ProjectSettingsModel {
				RegistrationEnabled = settings.GetValue<bool>(ProjectSettingKeys.RegistrationEnabled)
			};
		}

	}
}