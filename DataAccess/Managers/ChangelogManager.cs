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
	public class ChangelogManager : IManager<Changelog>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChangelogManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public ChangelogManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Changelog by ID
		/// </summary>
		/// <param name="id">Changelog ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Changelog> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Changelog>(StatusCode.NotValidID);
			}
			Changelog c = _ent.Changelogs.Find(id);
			if (c == null) {
				return new MActionResult<Changelog>(StatusCode.NotFound);
			}
			return new MActionResult<Changelog>(StatusCode.OK, c);
		}

		/// <summary>
		/// Changelogs the of the selected project.
		/// </summary>
		/// <param name="projectID">The owner Project ID.</param>
		/// <returns></returns>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<List<Changelog>> ChangelogsOfProject(int projectID)
		{
			if (!_mgr.Projects.ValidID(projectID)) {
				return new MActionResult<List<Changelog>>(StatusCode.NotValidID);
			}

			List<Changelog> c = _ent.Changelogs.Where(x => x.ProjectID == projectID).OrderBy(x => x.Date).ToList();
			return new MActionResult<List<Changelog>>(StatusCode.OK, c);
		}

		/// <summary>
		/// Changelogs the of the selected project.
		/// </summary>
		/// <param name="p">The owner Project ID.</param>
		/// <returns></returns>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<List<Changelog>> ChangelogsOfProject(Projects p)
		{
			return ChangelogsOfProject((int)p);
		}


		/// <summary>
		/// Creates Changelog
		/// </summary>
		/// <param name="projectID">Owner project ID</param>
		/// <param name="title">Title of changelog</param>
		/// <param name="text">Content of changelog</param>
		/// <param name="releaseDate">The release date of new version.</param>
		/// <param name="versionName">Name of the version.</param>
		/// <param name="visible">Visibility of changelog</param>
		/// <returns></returns>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Changelog> Create(int projectID, string title, string text, DateTime releaseDate, string versionName, bool visible = true)
		{
			if (!_mgr.Projects.ValidID(projectID)) {
				return new MActionResult<Changelog>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(versionName)) {
				return new MActionResult<Changelog>(StatusCode.InvalidInput);
			}
			Changelog c = new Changelog()
			{
				ProjectID = projectID,
				Title = title,
				Text = text,
				Date = releaseDate,
				Version = versionName,
				Visible = visible,
			};
			Changelog c1 = _ent.Changelogs.Add(c);
			Save(null);
			return new MActionResult<Changelog>(StatusCode.OK, c1);
		}

		/// <summary>
		/// Creates Changelog
		/// </summary>
		/// <param name="p">Owner project ID</param>
		/// <param name="title">Title of changelog</param>
		/// <param name="text">Content of changelog</param>
		/// <param name="releaseDate">The release date of new version.</param>
		/// <param name="versionName">Name of the version.</param>
		/// <param name="visible">Visibility of changelog</param>
		/// <returns></returns>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Changelog> Create(Projects p, string title, string text, DateTime releaseDate, string versionName, bool visible = true)
		{
			return Create((int)p, title, text, releaseDate, versionName, visible);
		}



		/// <summary>
		/// Saves Changelog
		/// </summary>
		/// <param name="c">The changelog to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(Changelog c, bool throwException = true)
		{
			try {
				if (c != null) {
					_ent.Changelogs.AddOrUpdate(c);
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
