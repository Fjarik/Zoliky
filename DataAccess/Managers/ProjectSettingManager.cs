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
using Newtonsoft.Json;
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

#region Get

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

		public async Task<string> GetStringValueAsync(Projects project, string key)
		{
			if (string.IsNullOrWhiteSpace(key)) {
				return null;
			}
			var res = await _ctx.ProjectSettings
								.AsProjectSettings((int) project, key)
								.Select(x => x.Value)
								.FirstOrDefaultAsync();
			return res;
		}

		public async Task<T> GetValueAsync<T>(Projects project, string key) where T : struct
		{
			var res = await this.GetStringValueAsync(project, key);
			if (string.IsNullOrWhiteSpace(res)) {
				return default;
			}
			return JsonConvert.DeserializeObject<T>(res);
		}

		public async Task<bool> GetBoolAsync(Projects project, string key)
		{
			var res = await this.GetStringValueAsync(project, key);
			if (string.IsNullOrWhiteSpace(res) || !bool.TryParse(res, out bool b)) {
				return false;
			}
			return b;
		}

		public async Task<int> GetIntAsync(Projects project, string key)
		{
			var res = await this.GetStringValueAsync(project, key);
			if (string.IsNullOrWhiteSpace(res) || !int.TryParse(res, out int i)) {
				return Ext.IgnoreId;
			}
			return i;
		}

#endregion

#region Edit and Save

		public Task<MActionResult<ProjectSetting>> EditAndSaveAsync(Projects project,
																	string key,
																	object newValue)
		{
			var s = JsonConvert.SerializeObject(newValue);
			return EditAndSaveAsync(project, key, s);
		}

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

		public Task<MActionResult<ProjectSetting>> CreateAsync(Projects project,
															   string key,
															   object value,
															   bool editIfExists = false)
		{
			var s = JsonConvert.SerializeObject(value);
			return CreateAsync(project, key, s, editIfExists);
		}

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

#region Remove

		public async Task<bool> RemoveAsync(int projectId,
											string key)
		{
			if (!Enum.IsDefined(typeof(Projects), projectId)) {
				return false;
			}
			return await RemoveAsync((Projects) projectId, key);
		}

		public async Task<bool> RemoveAsync(Projects project,
											string key)
		{
			var res = await this.GetAsync(project, key);
			if (!res.IsSuccess) {
				return false;
			}
			var set = res.Content;
			return await base.DeleteAsync(set);
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