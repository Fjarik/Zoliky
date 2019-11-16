using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class StatisticsManager : UserSettingManager
	{
		/// 
		/// Constructors
		/// 
		public StatisticsManager(IOwinContext context) : base(context) { }

		public StatisticsManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Own methods
		/// 

#region Static methods

		public static StatisticsManager Create(IdentityFactoryOptions<StatisticsManager> options,
											   IOwinContext context)
		{
			return new StatisticsManager(context);
		}

#endregion

		public async Task<List<UserSetting>> GetAllAsync(int userId)
		{
			return await _ctx.UserSettings
							 .Where(x => x.UserID == userId &&
										 SettingKeys.StatisticsKeys.Contains(x.Key))
							 .ToListAsync();
		}

		public async Task<MActionResult<UserSetting>> IncreaseValueAsync(int userId,
																		 string key,
																		 int step = 1,
																		 Projects? project = null)
		{
			if (userId < 1) {
				return new MActionResult<UserSetting>(StatusCode.NotValidID);
			}
			if (step == 0 || string.IsNullOrWhiteSpace(key)) {
				return new MActionResult<UserSetting>(StatusCode.InvalidInput);
			}
			int newVal = step;

			var exists = await base.ExistsAsync(userId, key, project);
			if (exists) {
				var oldVal = await base.GetIntAsync(userId, key, project);
				newVal = oldVal + step;
			}
			return await base.CreateAsync(userId, key, newVal.ToString(), true, project);
		}

#region Own Methods

#endregion

#endregion
	}
}