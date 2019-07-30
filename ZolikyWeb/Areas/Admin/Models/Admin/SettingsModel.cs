using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess.Models;
using SharedLibrary.Shared;
using ZolikyWeb.Areas.Admin.Models.Admin.SettingsModels;

namespace ZolikyWeb.Areas.Admin.Models.Admin
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