using System;
using System.Data.Entity;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class RankManager : Manager<Rank>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public RankManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public RankManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

#region Overrride

		public override Task<bool> DeleteAsync(Rank entity)
		{
			throw new NotSupportedException();
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static RankManager Create(IdentityFactoryOptions<RankManager> options,
										 IOwinContext context)
		{
			return new RankManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<Rank>> SelectAsync(int xp)
		{
			if (xp < 0) {
				return new MActionResult<Rank>(StatusCode.InvalidInput);
			}

			var res = await GetFromXP(xp);
			if (res == null) {
				return new MActionResult<Rank>(StatusCode.NotFound);
			}

			return new MActionResult<Rank>(StatusCode.OK, res);
		}

		public async Task<string> GetTitleAsync(int xp)
		{
			if (xp < 0) {
				return string.Empty;
			}

			var res = await GetFromXP(xp);
			if (res == null) {
				return string.Empty;
			}

			return res.Title;
		}

		private Task<Rank> GetFromXP(int xp)
		{
			return _ctx.Ranks.SingleOrDefaultAsync(x => x.FromXP <= xp && xp <= (x.ToXP ?? int.MaxValue));
		}

#endregion

#endregion
	}
}