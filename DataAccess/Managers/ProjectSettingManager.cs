using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class ProjectSettingManager : BaseManager<ProjectSetting>, IDisposable
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public ProjectSettingManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public ProjectSettingManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Own methods
		/// 

#region Static methods

		public static ProjectSettingManager Create(IdentityFactoryOptions<ProjectSettingManager> options,
												   IOwinContext context)
		{
			return new ProjectSettingManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<ProjectSetting>> GetAsync(Projects project, string key)
		{
			if (string.IsNullOrWhiteSpace(key)) {
				return new MActionResult<ProjectSetting>(StatusCode.InvalidInput);
			}

			var res = await _ctx.ProjectSettings
								.AsProjectSettings((int) project, key)
								.FirstOrDefaultAsync();
			if (res == null) {
				return new MActionResult<ProjectSetting>(StatusCode.NotFound);
			}
			return new MActionResult<ProjectSetting>(StatusCode.OK, res);
		}

#region Edit and Save

		public async Task<MActionResult<ProjectSetting>> EditAndSaveAsync(Projects project,
																		  string key,
																		  string newValue)
		{
			var res = await GetAsync(project, key);
			if (!res.IsSuccess) {
				return res;
			}
			var content = res.Content;
			return await EditAndSaveAsync(content, newValue);
		}

		private async Task<MActionResult<ProjectSetting>> EditAndSaveAsync(ProjectSetting setting,
																		   string newValue)
		{
			if (setting == null || string.IsNullOrWhiteSpace(newValue)) {
				return new MActionResult<ProjectSetting>(StatusCode.InvalidInput);
			}
			setting.Value = newValue;
			setting.Changed = DateTime.Now;
			await SaveAsync(setting);
			return new MActionResult<ProjectSetting>(StatusCode.OK, setting);
		}

#endregion

#region Create

		public async Task<MActionResult<ProjectSetting>> CreateAsync(Projects project,
																	 string key,
																	 string value,
																	 bool editIfExists = false)
		{
			if (Methods.AreNullOrWhiteSpace(key, value)) {
				return new MActionResult<ProjectSetting>(StatusCode.InvalidInput);
			}
			if ((await this.ExistsAsync(project, key))) {
				if (!editIfExists) {
					return new MActionResult<ProjectSetting>(StatusCode.AlreadyExists);
				}
				return await this.EditAndSaveAsync(project, key, value);
			}

			var us = new ProjectSetting() {
				ProjectID = (int) project,
				Key = key,
				Value = value,
				Changed = DateTime.Now
			};
			return await CreateAsync(us);
		}

		private async Task<MActionResult<ProjectSetting>> CreateAsync(ProjectSetting setting)
		{
			if (setting == null) {
				return new MActionResult<ProjectSetting>(StatusCode.InvalidInput);
			}
			var ent = _ctx.ProjectSettings.Add(setting);
			var changes = await base.SaveAsync();
			if (changes == 0) {
				return new MActionResult<ProjectSetting>(StatusCode.InternalError);
			}
			return new MActionResult<ProjectSetting>(StatusCode.OK, ent);
		}

#endregion

#region Exists

		public Task<bool> ExistsAsync(Projects project, string key)
		{
			return ExistsAsync((int) project, key);
		}

		public Task<bool> ExistsAsync(int projectId, string key)
		{
			return _ctx.ProjectSettings.AnyAsync(x => x.ProjectID == projectId &&
													  x.Key == key);
		}

#endregion

#endregion

#endregion
	}
}