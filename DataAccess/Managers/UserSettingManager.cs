using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class UserSettingManager : Manager<UserSetting>
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

#region Overrides

		public override Task<bool> DeleteAsync(int id)
		{
			throw new NotImplementedException("Použijte metodu RemoveAsync");
		}

#endregion

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
			var query = _ctx.UserSettings.Where(x => x.ID == userId);
			if (project != null) {
				query = query.Where(x => x.ProjectId == (int) project);
			}
			var res = await query.ToListAsync();

			return new MActionResult<List<UserSetting>>(StatusCode.OK, res);
		}

		public Task<MActionResult<UserSetting>> EditAndSaveAsync(int userId,
																 string key,
																 object newValue,
																 Projects? project = null)
		{
			return this.EditAndSaveAsync(userId, key, newValue.ToString(), project);
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

		public Task<MActionResult<UserSetting>> CreateAsync(int userId,
															string key,
															object value,
															bool editIfExists = false,
															Projects? project = null)
		{
			return CreateAsync(userId, key, value.ToString(), editIfExists, project);
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
				ID = userId,
				ProjectId = (int?) project,
				Key = key,
				Value = value,
				Changed = DateTime.Now
			};
			return await CreateAsync(us);
		}

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
			return res.Convert<T>();
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

		public Task<bool> ExistsAsync(int userId, string key, Projects? project = null)
		{
			return this.ExistsAsync(userId, key, (int?) project);
		}

		public Task<bool> ExistsAsync(int userId, string key, int? projectId = null)
		{
			return _ctx.UserSettings.AnyAsync(x => x.ID == userId &&
												   x.Key == key &&
												   x.ProjectId == projectId);
		}

#endregion

#endregion
	}
}