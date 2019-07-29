using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class UserSettingManager : BaseManager<UserSetting>, IDisposable
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public UserSettingManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public UserSettingManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Own methods
		/// 

#region Static methods

		public static UserSettingManager Create(IdentityFactoryOptions<UserSettingManager> options,
												IOwinContext context)
		{
			return new UserSettingManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<List<UserSetting>>> GetAllByUserIdAsync(int userId,
																				Projects? project = null)
		{
			if (userId < 1) {
				return new MActionResult<List<UserSetting>>(StatusCode.NotValidID);
			}
			var query = _ctx.UserSettings.Where(x => x.UserID == userId);
			if (project != null) {
				query = query.Where(x => x.ProjectID == (int) project);
			}
			var res = await query.ToListAsync();

			return new MActionResult<List<UserSetting>>(StatusCode.OK, res);
		}

#region Edit and save

		public Task<MActionResult<UserSetting>> EditAndSaveAsync(int userId,
																 string key,
																 object newValue,
																 Projects? project = null)
		{
			var json = JsonConvert.SerializeObject(newValue);
			return this.EditAndSaveAsync(userId, key, json, project);
		}

		public async Task<MActionResult<UserSetting>> EditAndSaveAsync(int userId,
																	   string key,
																	   string newValue,
																	   Projects? project = null)
		{
			var res = await this.GetAsync(userId, key, project);
			if (!res.IsSuccess) {
				return res;
			}
			var content = res.Content;
			return await EditAndSaveAsync(content, newValue);
		}

		private async Task<MActionResult<UserSetting>> EditAndSaveAsync(UserSetting setting,
																		string newValue)
		{
			if (setting == null || string.IsNullOrWhiteSpace(newValue)) {
				return new MActionResult<UserSetting>(StatusCode.InvalidInput);
			}
			setting.Value = newValue;
			setting.Changed = DateTime.Now;
			await SaveAsync(setting);
			return new MActionResult<UserSetting>(StatusCode.OK, setting);
		}

#endregion

#region Create

		public Task<MActionResult<UserSetting>> CreateAsync(int userId,
															string key,
															object value,
															bool editIfExists = false,
															Projects? project = null)
		{
			var json = JsonConvert.SerializeObject(value);
			return CreateAsync(userId, key, json, editIfExists, project);
		}

		public async Task<MActionResult<UserSetting>> CreateAsync(int userId,
																  string key,
																  string value,
																  bool editIfExists = false,
																  Projects? project = null)
		{
			if (userId < 1) {
				return new MActionResult<UserSetting>(StatusCode.NotValidID);
			}
			if (Methods.AreNullOrWhiteSpace(key, value)) {
				return new MActionResult<UserSetting>(StatusCode.InvalidInput);
			}
			if ((await this.ExistsAsync(userId, key, project))) {
				if (!editIfExists) {
					return new MActionResult<UserSetting>(StatusCode.AlreadyExists);
				}
				return await this.EditAndSaveAsync(userId, key, value, project);
			}

			var us = new UserSetting() {
				UserID = userId,
				ProjectID = (int?) project,
				Key = key,
				Value = value,
				Changed = DateTime.Now
			};
			return await CreateAsync(us);
		}

		private async Task<MActionResult<UserSetting>> CreateAsync(UserSetting setting)
		{
			if (setting == null) {
				return new MActionResult<UserSetting>(StatusCode.InvalidInput);
			}
			var ent = _ctx.UserSettings.Add(setting);
			var changes = await base.SaveAsync();
			if (changes == 0) {
				return new MActionResult<UserSetting>(StatusCode.InternalError);
			}
			return new MActionResult<UserSetting>(StatusCode.OK, ent);
		}

#endregion

#region Remove

		public async Task<bool> RemoveAsync(int userId,
											string key,
											Projects? project = null)
		{
			var res = await GetAsync(userId, key, project);
			if (!res.IsSuccess) {
				return false;
			}
			var set = res.Content;
			return await this.DeleteAsync(set);
		}

#endregion

#region Get

		public async Task<MActionResult<UserSetting>> GetAsync(int userId, string key, Projects? project = null)
		{
			if (userId < 1) {
				return new MActionResult<UserSetting>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(key)) {
				return new MActionResult<UserSetting>(StatusCode.InvalidInput);
			}

			var res = await _ctx.UserSettings
								.AsUserSettings(userId, key, (int?) project)
								.FirstOrDefaultAsync();
			if (res == null) {
				return new MActionResult<UserSetting>(StatusCode.NotFound);
			}
			return new MActionResult<UserSetting>(StatusCode.OK, res);
		}

		public async Task<string> GetStringValueAsync(int userId, string key, Projects? project = null)
		{
			if (userId < 1 || string.IsNullOrWhiteSpace(key)) {
				return null;
			}
			var res = await _ctx.UserSettings
								.AsUserSettings(userId, key, (int?) project)
								.Select(x => x.Value)
								.FirstOrDefaultAsync();
			return res;
		}

		public async Task<T> GetValueAsync<T>(int userId, string key, Projects? project = null) where T : struct
		{
			var res = await this.GetStringValueAsync(userId, key, project);
			if (string.IsNullOrWhiteSpace(res)) {
				return default;
			}
			return JsonConvert.DeserializeObject<T>(res);
		}

		public async Task<bool> GetBoolAsync(int userId, string key, Projects? project = null)
		{
			var res = await this.GetStringValueAsync(userId, key, project);
			if (string.IsNullOrWhiteSpace(res) || !bool.TryParse(res, out bool b)) {
				return false;
			}
			return b;
		}

		public async Task<int> GetIntAsync(int userId, string key, Projects? project = null)
		{
			var res = await this.GetStringValueAsync(userId, key, project);
			if (string.IsNullOrWhiteSpace(res) || !int.TryParse(res, out int i)) {
				return Ext.IgnoreId;
			}
			return i;
		}

#endregion

#region Exists

		public Task<bool> ExistsAsync(int userId, string key, Projects? project = null)
		{
			return this.ExistsAsync(userId, key, (int?) project);
		}

		public Task<bool> ExistsAsync(int userId, string key, int? projectId = null)
		{
			return _ctx.UserSettings.AnyAsync(x => x.UserID == userId &&
												   x.Key == key &&
												   x.ProjectID == projectId);
		}

#endregion

#endregion

#endregion
	}
}