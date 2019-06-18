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
	public class ProjectManager : IManager<Project>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public ProjectManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Project by ID
		/// </summary>
		/// <param name="id">Project ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.NotEnabled" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Project> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Project>(StatusCode.NotValidID);
			}

			Project p = _ent.Projects.Find(id);
			if (p == null) {
				return new MActionResult<Project>(StatusCode.NotFound);
			}

			if (!p.Active) {
				return new MActionResult<Project>(StatusCode.NotEnabled, p);
			}
			return new MActionResult<Project>(StatusCode.OK, p);
		}

		/// <summary>
		/// Selects Project by ID
		/// </summary>
		/// <param name="p">Project ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.NotEnabled" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Project> Select(Projects p)
		{
			return Select((int)p);
		}

		/// <summary>
		/// Gets all projects
		/// </summary>
		/// <exception cref="StatusCode.OK" />
		public MActionResult<List<Project>> GetAll()
		{
			List<Project> p = _ent.Projects.Where(x => x.Active).ToList();
			return new MActionResult<List<Project>>(StatusCode.OK, p);
		}

		/// <summary>
		/// Saves project.
		/// </summary>
		/// <param name="p">The Project to save</param>
		/// <param name="throwException">if set to <c>true</c> [throw exception].</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public int Save(Project p, bool throwException = true)
		{
			try {
				if (p != null) {
					_ent.Projects.AddOrUpdate(p);
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

		/// <summary>
		/// Check if ID is valid
		/// </summary>
		/// <param name="id">Project ID</param>
		/// <returns></returns>
		public bool ValidID(int id)
		{
			if (id < 1) {
				return false;
			}
			return _ent.Projects.Any(x => x.ID == id);
		}

	}
}
