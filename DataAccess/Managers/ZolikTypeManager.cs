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
	public class ZolikTypeManager : Manager<ZolikType>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public ZolikTypeManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public ZolikTypeManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

		public override Task<bool> DeleteAsync(ZolikType entity)
		{
			throw new NotSupportedException("Cannot delete directly");
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static ZolikTypeManager Create(IdentityFactoryOptions<ZolikTypeManager> options,
											  IOwinContext context)
		{
			return new ZolikTypeManager(context);
		}

#endregion

#region Own Methods

		public Task<List<ZolikType>> GetAll(bool isTester = false)
		{
			if (isTester) {
				return this._ctx.ZolikType.ToListAsync();
			}
			return this._ctx.ZolikType.Where(x => !x.IsTestType).ToListAsync();
		}

		public async Task<MActionResult<ZolikType>> CreateAsync(string name,
																string fName,
																bool isSplittable,
																bool isTestType,
																bool allowGive,
																int? splitsTo = null)
		{
			if (splitsTo != null) {
				if (splitsTo < 1) {
					return new MActionResult<ZolikType>(StatusCode.NotValidID);
				}
				if (!isSplittable) {
					// If splits to -> Must be splittable
					return new MActionResult<ZolikType>(StatusCode.InvalidInput);
				}
			}
			var t = new ZolikType() {
				Name = name,
				FriendlyName = fName,
				IsSplittable = isSplittable,
				SplitsToID = splitsTo,
				IsTestType = isTestType,
				AllowGive = allowGive,
			};
			return await this.CreateAsync(t);
		}

#endregion

#endregion
	}
}