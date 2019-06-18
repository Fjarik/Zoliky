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

namespace DataAccess.Managers.New
{
	public class UnavailabilityManager : Manager<Unavailability>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public UnavailabilityManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public UnavailabilityManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

#region Overrides

		public override Task<bool> DeleteAsync(Unavailability entity)
		{
			throw new NotSupportedException("Mazání odstávek není možné, použijte metodu na dokončení");
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static UnavailabilityManager Create(IdentityFactoryOptions<UnavailabilityManager> options,
												   IOwinContext context)
		{
			return new UnavailabilityManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<Unavailability>> CreateAsync(Projects project, string reason, DateTime from,
																	 DateTime to, string desc = "")
		{
			if (string.IsNullOrWhiteSpace(reason) || to < from) {
				return new MActionResult<Unavailability>(StatusCode.InvalidInput);
			}

			var ent = new Unavailability() {
				ProjectID = (int) project,
				Reason = reason,
				Description = desc,
				From = from,
				To = to
			};
			return await base.CreateAsync(ent);
		}

		public MActionResult<Unavailability> GetFirst(
			Projects project, DateTime?  when = null)
		{
			var res = (when == null ? this.GetCurrent(project) : _ctx.GetUnavailabilities(project, when)).First();

			if (res == null) {
				return new MActionResult<Unavailability>(StatusCode.NotFound);
			}
			return new MActionResult<Unavailability>(StatusCode.OK, res);
		}

		public  MActionResult<List<Unavailability>> GetAll(
			Projects project, DateTime? when = null)
		{
			var res = (when == null ? this.GetCurrent(project) : _ctx.GetUnavailabilities(project, when)).ToList();

			if (!res.Any()) {
				return new MActionResult<List<Unavailability>>(StatusCode.NotFound);
			}
			return new MActionResult<List<Unavailability>>(StatusCode.OK, res);
		}

		private IList<Unavailability> GetCurrent(Projects project)
		{
			return _ctx.GetUnavailabilities(project);
		}

		public async Task<bool> IsAvailableAsync(Projects project)
		{
			return !(await IsUnavailableAsync(project));
		}

		public Task<bool> IsUnavailableAsync(Projects project)
		{
			return _ctx.Unavailabilities
					   .IsActive()
					   .AnyAsync(x => x.ProjectID == (int)project);
		}

#endregion

#endregion
	}
}