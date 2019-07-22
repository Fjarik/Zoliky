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
	public class UserLogManager : Manager<UserLog>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public UserLogManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public UserLogManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

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

		public static UserLogManager Create(IdentityFactoryOptions<UserLogManager> options,
											IOwinContext context)
		{
			return new UserLogManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<UserLog>> CreateAsync(int userId, string text)
		{
			if (userId < 1) {
				return new MActionResult<UserLog>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(text)) {
				return new MActionResult<UserLog>(StatusCode.InvalidInput);
			}

			var ent = new UserLog {
				UserID = userId,
				Event = text,
				DateCreated = DateTime.Now
			};
			return await base.CreateAsync(ent);
		}

#endregion

#endregion
	}
}