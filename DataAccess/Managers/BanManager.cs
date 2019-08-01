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

namespace DataAccess.Managers
{
	public class BanManager : Manager<Ban>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public BanManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public BanManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static BanManager Create(IdentityFactoryOptions<BanManager> options,
										IOwinContext context)
		{
			return new BanManager(context);
		}

#endregion

#region Own Methods

		public Task<bool> IsBannedAsync(int userId)
		{
			return _ctx.Bans
					   .Where(Extensions.IsActive())
					   .AnyAsync(x => x.UserID == userId);
		}

		public Task<Ban> GetActiveBanAsync(int userId)
		{
			return _ctx.Bans
					   .Where(Extensions.IsActive())
					   .FirstOrDefaultAsync(x => x.UserID == userId);
		}

		public Task<List<Ban>> GetActiveBansAsync()
		{
			return _ctx.Bans
					   .Where(Extensions.IsActive())
					   .ToListAsync();
		}

		public async Task<MActionResult<Ban>> CreateAsync(string reason,
														  int userId,
														  DateTime? to = null)
		{
			var from = DateTime.Now;

			if (userId < 1) {
				return new MActionResult<Ban>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(reason) || (to != null && to < from)) {
				return new MActionResult<Ban>(StatusCode.InvalidInput);
			}

			var ent = new Ban() {
				UserID = userId,
				From = from,
				To = to,
				Reason = reason
			};
			return await base.CreateAsync(ent);
		}

		public async Task<bool> UnbanAsync(int userId)
		{
			var elm = await this.GetActiveBanAsync(userId);
			if (elm == null) {
				return false;
			}
			return await DeleteAsync(elm);
		}

#endregion

#endregion
	}
}