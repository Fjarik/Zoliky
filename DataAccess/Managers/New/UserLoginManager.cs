using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers.New
{
	public class UserLoginManager : Manager<UserLogin>, IDisposable
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public UserLoginManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public UserLoginManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

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

		public static UserLoginManager Create(IdentityFactoryOptions<UserLoginManager> options, IOwinContext context)
		{
			return new UserLoginManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<UserLogin>> CreateAsync(int userId,
																Projects project,
																LoginStatus status,
																string ip)
		{
			UserManager mgr = Context.Get<UserManager>();
			if (!await mgr.IdExistsAsync(userId)) {
				return new MActionResult<UserLogin>(StatusCode.NotValidID);
			}

			if (ip == "::1") {
				ip = "127.0.0.1";
			}

			UserLogin uLogin = new UserLogin() {
				UserID = userId,
				ProjectID = (int) project,
				Date = DateTime.Now,
				Status = status,
				IP = ip,
			};
			return await this.CreateAsync(uLogin);
		}

		public async Task<MActionResult<List<UserLogin>>> GetAllAsync(int userId, int count = 100, LoginStatus? status = null)
		{
			if (userId < 1) {
				return new MActionResult<List<UserLogin>>(StatusCode.NotValidID);
			}
			var query = _ctx.UserLogins.AsQueryable();
			if (status != null) {
				query = query.Where(x => x.Status == status);
			}
			var res = await query.Where(x => x.UserID == userId)
								 .OrderByDescending(x => x.Date)
								 .Take(count)
								 .ToListAsync();

			return new MActionResult<List<UserLogin>>(StatusCode.OK, res);
		}

		public async Task<int> GetCountAsync(int userId, LoginStatus? status = null)
		{
			if (userId < 1) {
				return 0;
			}
			var query = _ctx.UserLogins.AsQueryable();
			if (status != null) {
				query = query.Where(x => x.Status == status);
			}
			return await query.CountAsync(x => x.UserID == userId);
		}

#endregion

#endregion
	}
}