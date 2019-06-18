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
	public class ChangelogManager : Manager<Changelog>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public ChangelogManager(IOwinContext context) : this(context, new ZoliksEntities())
		{
		}

		public ChangelogManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx)
		{
		}

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

		public static ChangelogManager Create(IdentityFactoryOptions<ChangelogManager> options, IOwinContext context)
		{
			return new ChangelogManager(context);
		}

#endregion

#region Own Methods

		public Task<List<Changelog>> GetAllAsync(Projects project)
		{
			return GetAllAsync((int) project);
		}

		private async Task<List<Changelog>> GetAllAsync(int projectId)
		{
			var c = await _ctx.Changelogs
				.AsNoTracking()
				.Where(x => x.ProjectID == projectId)
				.ToListAsync();
			return c;
		}

		public Task<MActionResult<Changelog>> CreateAsync(int projectId,
			string title,
			string text,
			string version,
			bool visible = true)
		{
			return CreateAsync(projectId, title, text, version, DateTime.Now, visible);
		}

		public async Task<MActionResult<Changelog>> CreateAsync(int projectId,
			string title,
			string text,
			string version,
			DateTime publish,
			bool visible = true)
		{
			if (!Enum.IsDefined(typeof(Projects), projectId)) {
				return new MActionResult<Changelog>(StatusCode.NotValidID);
			}

			if (Methods.AreNullOrWhiteSpace(title, text, version)) {
				return new MActionResult<Changelog>(StatusCode.InvalidInput);
			}

			Changelog c = new Changelog()
			{
				ProjectID = projectId,
				Title = title,
				Text = text,
				Version = version,
				Visible = visible,
				Date = publish
			};
			return await this.CreateAsync(c);
		}

#endregion

#endregion
	}
}