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
using SharedLibrary.Shared.Objects;

namespace DataAccess.Managers
{
	public class AchievementManager : Manager<Achievement>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public AchievementManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public AchievementManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

		public override Task<List<Achievement>> GetAllAsync()
		{
			return _ctx.Achievements.Where(x => x.Enabled).ToListAsync();
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static AchievementManager Create(IdentityFactoryOptions<AchievementManager> options,
												IOwinContext context)
		{
			return new AchievementManager(context);
		}

#endregion

#region Own Methods

		public async Task<List<AchievementModel>> GetUserAchievementModels(int userId)
		{
			if (userId < 1) {
				return new List<AchievementModel>();
			}

			var achs = await this.GetAllAsync();
			var unlocked = await this.GetUnlockedIdsAsync(userId);

			var ids = unlocked.Select(x => x.AchievementId);

			var model = achs.Where(x => ids.All(y => x.ID != y))
							.Select(x => new AchievementModel(x));

			var un = unlocked.Select(x => new AchievementModel(achs.FirstOrDefault(y => y.ID == x.AchievementId)) {
										 IsUnlocked = true,
										 When = x.When
									 }
									);

			var res = un.Concat(model);

			return res.ToList();
		}

		public async Task<IList<AchievementUnlock>> GetUnlockedIdsAsync(int userId)
		{
			if (userId < 1) {
				return new List<AchievementUnlock>();
			}
			return await _ctx.AchievementUnlocks
							 .Where(x => x.UserId == userId)
							 .ToListAsync();
		}

		public Task<bool> IsUnlockedAsync(int userId, int achId)
		{
			return _ctx.AchievementUnlocks
					   .AnyAsync(x => x.UserId == userId &&
									  x.AchievementId == achId);
		}

		public async Task<MActionResult<AchievementUnlock>> UnlockAsync(int userId, int achId)
		{
			if (userId < 1 || achId < 1) {
				return new MActionResult<AchievementUnlock>(StatusCode.NotValidID);
			}
			if ((await IsUnlockedAsync(userId, achId))) {
				return new MActionResult<AchievementUnlock>(StatusCode.AlreadyExists);
			}
			var ent = new AchievementUnlock() {
				UserId = userId,
				AchievementId = achId,
				When = DateTime.Now
			};
			return await this.CreateUnlockAsync(ent);
		}

		public async Task<MActionResult<Achievement>> CreateAsync(string title,
																  string desc,
																  int xp,
																  int? valToUnlock = null)
		{
			if (Methods.AreNullOrWhiteSpace(title, desc) || xp < 0) {
				return new MActionResult<Achievement>(StatusCode.InvalidInput);
			}

			var ent = new Achievement() {
				Title = title,
				Description = desc,
				XP = xp,
				ValueToUnlock = valToUnlock,
				Enabled = true
			};
			return await base.CreateAsync(ent);
		}

		private async Task<MActionResult<AchievementUnlock>> CreateUnlockAsync(AchievementUnlock unlock)
		{
			var ent = _ctx.AchievementUnlocks.Add(unlock);
			var changes = await base.SaveAsync();
			if (changes == 0) {
				return new MActionResult<AchievementUnlock>(StatusCode.InternalError);
			}
			ent = await _ctx.AchievementUnlocks
							.AsNoTracking()
							.Include(x => x.Achievement)
							.FirstOrDefaultAsync(x => x.UserId == ent.UserId &&
													  x.AchievementId == ent.AchievementId);
			return new MActionResult<AchievementUnlock>(StatusCode.OK, ent);
		}

		public async Task<bool> CheckAsync(int userId)
		{
			var sMgr = Context.Get<UserSettingManager>();
			var achs = await this.GetAllAsync();

			var res = false;
			foreach (var ach in achs) {
				var check = await CheckAsync(sMgr, userId, ach);
				if (check) {
					res = true;
				}
			}
			return res;
		}

		private async Task<bool> CheckAsync(UserSettingManager mgr, int userId, Achievement ach)
		{
			if (userId < 1 || ach == null || mgr == null || ach.ValueToUnlock == null) {
				return false;
			}
			var val = await mgr.GetIntAsync(userId, ach.RelatedKey);
			if (val < 1 || val < ach.ValueToUnlock) {
				return false;
			}
			var res = await this.UnlockAsync(userId, ach.ID);
			return res.IsSuccess;
		}

#endregion

#endregion
	}
}