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

		public async Task<MActionResult<Achievement>> CreateAsync(string title,
																  string desc,
																  int xp,
																  int? lockedId = null,
																  int? unlockedId = null)
		{
			if (Methods.AreNullOrWhiteSpace(title, desc) || xp < 0) {
				return new MActionResult<Achievement>(StatusCode.InvalidInput);
			}
			if ((lockedId != null && lockedId < 1) || (unlockedId != null && unlockedId < 1)) {
				return new MActionResult<Achievement>(StatusCode.NotValidID);
			}

			var ent = new Achievement() {
				ImageLockedID = lockedId,
				UnlockedImageID = unlockedId,
				Title = title,
				Description = desc,
				XP = xp,
				Enabled = true
			};
			return await base.CreateAsync(ent);
		}

#endregion

#endregion
	}
}