using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using DataAccess.Errors;
using DataAccess.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class NewsManager : IManager<News>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="NewsManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public NewsManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects News by ID
		/// </summary>
		/// <param name="id">News ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<News> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<News>(StatusCode.NotValidID);
			}
			News n = _ent.News.Find(id);
			if (n == null) {
				return new MActionResult<News>(StatusCode.NotFound);
			}
			return new MActionResult<News>(StatusCode.OK, n);
		}

		/// <summary>
		/// Gets all News.
		/// </summary>
		public MActionResult<List<News>> GetAll()
		{
			List<News> n = _ent.News.Where(x => x.Expiration == null || x.Expiration > DateTime.Now)
				.OrderBy(x => x.Created).ToList();
			return new MActionResult<List<News>>(StatusCode.OK, n);
		}

		/// <summary>
		/// Creates News
		/// </summary>
		/// <param name="title">The title of the News</param>
		/// <param name="message">The content of the News</param>
		/// <param name="exp">Expiration from NOW</param>
		/// <param name="loggedOnly">Only logged users can see this news</param>
		/// <param name="project">Project</param>
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<News> Create(string title, string message, TimeSpan? exp, bool loggedOnly, Projects? project)
		{
			if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message) || (exp != null && ((TimeSpan)exp).TotalMilliseconds < 1)) {
				return new MActionResult<News>(StatusCode.InvalidInput);
			}

			News n = new News() {
				ProjectID = (int?)project,
				Title = title,
				Message = message,
				Created = DateTime.Now,
				LoginOnly = loggedOnly
			};
			if (exp == null) {
				n.Expiration = null;
			} else {
				n.Expiration = DateTime.Now.Add((TimeSpan)exp);
			}
			News n1 = _ent.News.Add(n);
			Save(n1);
			return new MActionResult<News>(StatusCode.OK, n1);
		}

		/// <summary>
		/// Creates News
		/// </summary>
		/// <param name="title">The title of the News</param>
		/// <param name="message">The content of the News</param>
		/// <param name="loggedOnly">Only logged users can see this news</param>
		/// <param name="exp">Expiration date</param>
		/// <param name="project">Project</param>
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<News> Create(string title, string message, bool loggedOnly, DateTime? exp, Projects? project)
		{
			if (exp != null && DateTime.Now > exp) {
				return new MActionResult<News>(StatusCode.InvalidInput);
			}

			if (exp == null) {
				return Create(title, message, null, loggedOnly, project);
			}
			return Create(title, message, (DateTime)exp - DateTime.Now, loggedOnly, project);
		}

		/// <summary>
		/// Saves News
		/// </summary>
		/// <param name="n">The News to save</param>
		/// <param name="throwException">if set to <c>true</c> [throw exception].</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(News n, bool throwException = true)
		{
			try {
				if (n != null) {
					_ent.News.AddOrUpdate(n);
				}
				int changes = _ent.SaveChanges();
				return changes;
			} catch (DbEntityValidationException ex) {
				if (throwException) {
					throw new DbEntityValidationException(ex.GetExceptionMessage(), ex.EntityValidationErrors);
				}
				return 0;
			}
		}
	}
}
