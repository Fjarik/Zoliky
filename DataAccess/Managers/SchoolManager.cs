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
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class SchoolManager : Manager<School>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public SchoolManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public SchoolManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

		public override Task<bool> DeleteAsync(School entity)
		{
			throw new NotSupportedException("Školy nelze smazat");
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static SchoolManager Create(IdentityFactoryOptions<SchoolManager> options,
										   IOwinContext context)
		{
			return new SchoolManager(context);
		}

#endregion

#region Own Methods

#region Create

		public async Task<MActionResult<School>> CreateAsync(SchoolTypes type,
															 string name,
															 string street,
															 string city,
															 bool allowTransfer,
															 bool allowTeacherRemove,
															 bool allowSplit)
		{
			if (Methods.AreNullOrWhiteSpace(name, street, city)) {
				return new MActionResult<School>(StatusCode.InvalidInput);
			}

			var ent = new School {
				Type = type,
				Name = name,
				Street = street,
				City = city,
				AllowTransfer = allowTransfer,
				AllowTeacherRemove = allowTeacherRemove,
				AllowZolikSplik = allowSplit
			};
			return await base.CreateAsync(ent);
		}

#endregion

#endregion

#endregion
	}
}