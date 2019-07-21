using System;
using System.Collections.Generic;
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

#endregion

#endregion
	}
}