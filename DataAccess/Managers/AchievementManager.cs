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

#endregion

#endregion
	}
}