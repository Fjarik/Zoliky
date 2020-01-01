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

namespace DataAccess.Managers
{
	public class NewsManager : Manager<News>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public NewsManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public NewsManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

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

		public static NewsManager Create(IdentityFactoryOptions<NewsManager> options,
										 IOwinContext context)
		{
			return new NewsManager(context);
		}

#endregion

#region Own Methods

		public Task<List<News>> GetAllActive()
		{
			return _ctx.News
					   .Where(x => x.Expiration == null ||
								   x.Expiration > DateTime.Now)
					   .OrderByDescending(x => x.Created)
					   .ToListAsync();
		}

		public async Task<MActionResult<News>> CreateAsync(int authorId,
														   string title,
														   string message,
														   bool loginOnly = false,
														   DateTime? expiration = null,
														   Projects? project = null)
		{
			if (authorId < 1) {
				return new MActionResult<News>(StatusCode.NotValidID);
			}
			if (Methods.AreNullOrWhiteSpace(title, message)) {
				return new MActionResult<News>(StatusCode.InvalidInput);
			}
			if (expiration != null && expiration < DateTime.Now) {
				return new MActionResult<News>(StatusCode.InvalidInput);
			}

			var ent = new News() {
				AuthorID = authorId,
				Title = title,
				Message = message,
				Expiration = expiration,
				LoginOnly = loginOnly,
				ProjectID = (int?) project,
				Created = DateTime.Now,
				LastUpdate = DateTime.Now
			};
			return await base.CreateAsync(ent);
		}

#endregion

#endregion
	}
}