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
			return _ctx.Bans.AnyAsync(x => x.UserID == userId && x.IsActive);
		}

		public Task<bool> IsBannedAsync(string ip)
		{
			return _ctx.Bans.AnyAsync(x => x.IP == ip && x.IsActive);
		}

		public Task<Ban> GetActiveBanAsync(int userId)
		{
			return _ctx.Bans.FirstOrDefaultAsync(x => x.UserID == userId && x.IsActive);
		}

		public Task<Ban> GetActiveBanAsync(string ip)
		{
			return _ctx.Bans.FirstOrDefaultAsync(x => x.IP == ip && x.IsActive);
		}

		public Task<MActionResult<Ban>> BanUserAsync(int userId,
													 string reason,
													 DateTime? to = null)
		{
			return CreateAsync(reason,
							   userId: userId,
							   to: to);
		}

		public Task<MActionResult<Ban>> IpBanAsync(string ip,
												   string reason,
												   DateTime? to = null)
		{
			return CreateAsync(reason,
							   to: to,
							   ip: ip);
		}

		public async Task<MActionResult<Ban>> CreateAsync(string reason,
														  int? userId = null,
														  DateTime? to = null,
														  string ip = null)
		{
			var from = DateTime.Now;

			if (userId != null && userId < 1) {
				return new MActionResult<Ban>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(reason) || (to != null && to < from)) {
				return new MActionResult<Ban>(StatusCode.InvalidInput);
			}
			if (userId == null && string.IsNullOrWhiteSpace(ip)) {
				return new MActionResult<Ban>(StatusCode.InvalidInput);
			}

			var ent = new Ban() {
				UserID = userId,
				IP = ip,
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

		public async Task<bool> UnbanAsync(string ip)
		{
			var elm = await this.GetActiveBanAsync(ip);
			if (elm == null) {
				return false;
			}
			return await DeleteAsync(elm);
		}

#endregion

#endregion
	}
}