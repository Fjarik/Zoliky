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
	public class WebEventManager : IManager<WebEvent>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebEventManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public WebEventManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Web Event by ID.
		/// </summary>
		/// <param name="id">WebEvent ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.NotEnabled" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<WebEvent> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<WebEvent>(StatusCode.NotValidID);
			}

			WebEvent e = _ent.WebEvents.Find(id);
			if (e == null) {
				return new MActionResult<WebEvent>(StatusCode.NotFound);
			}
			if (!e.Enabled) {
				return new MActionResult<WebEvent>(StatusCode.NotEnabled, e);
			}
			return new MActionResult<WebEvent>(StatusCode.OK, e);
		}

		/// <summary>
		/// Gets all Web Events of User.
		/// </summary>
		/// <param name="toID">User ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<List<WebEvent>> GetAll(int toID)
		{
			if (toID < 1) {
				return new MActionResult<List<WebEvent>>(StatusCode.NotValidID);
			}

			List<WebEvent> events = _ent.WebEvents.AsNoTracking()
				.Where(x => x.ToID == toID &&
							x.Enabled)
				.ToList();
			return new MActionResult<List<WebEvent>>(StatusCode.OK, events);
		}

		/// <summary>
		/// Gets the newest Web Events of User.
		/// </summary>
		/// <param name="toID">User ID.</param>
		/// <param name="MinOld">The minimum date.</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<List<WebEvent>> GetNewest(int toID, DateTime MinOld)
		{
			if (toID < 1) {
				return new MActionResult<List<WebEvent>>(StatusCode.NotValidID);
			}

			List<WebEvent> events = _ent.WebEvents.AsNoTracking()
				.Where(x => x.ToID == toID &&
							x.Enabled &&
							x.Date > MinOld)
				.ToList();
			return new MActionResult<List<WebEvent>>(StatusCode.OK, events);

		}

		/// <summary>
		/// Creates new Web Event.
		/// </summary>
		/// <param name="project">The project.</param>
		/// <param name="from">From ID.</param>
		/// <param name="to">To ID.</param>
		/// <param name="type">The type of event.</param>
		/// <param name="msg">The message.</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<WebEvent> Create(Projects project, int? from, int to, int type, string msg)
		{
			if (to < 1 || type < 1) {
				return new MActionResult<WebEvent>(StatusCode.NotValidID);
			}

			WebEvent w = new WebEvent()
			{
				FromProjectID = (int)project,
				FromID = from,
				ToID = to,
				Type = type,
				Date = DateTime.Now,
				Enabled = true,
				Message = msg
			};
			WebEvent w2 = _ent.WebEvents.Add(w);
			Save(null);
			return new MActionResult<WebEvent>(StatusCode.OK, w2);
		}

		/// <summary>
		/// Saves Web Event
		/// </summary>
		/// <param name="content">The Web Event to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(WebEvent content, bool throwException = true)
		{
			try {
				if (content != null) {
					_ent.WebEvents.AddOrUpdate(content);
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
