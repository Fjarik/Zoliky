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
	public class LoginsManager : IManager<UserLogin>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginsManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public LoginsManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects UserLogin by ID.
		/// </summary>
		/// <param name="id">WebEvent ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<UserLogin> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<UserLogin>(StatusCode.NotValidID);
			}

			UserLogin l = _ent.UserLogins.Find(id);
			if (l == null) {
				return new MActionResult<UserLogin>(StatusCode.NotFound);
			}
			return new MActionResult<UserLogin>(StatusCode.OK, l);
		}

		/// <summary>
		/// Gets all Logins of User.
		/// </summary>
		/// <param name="user">User ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<List<UserLogin>> GetAll(int user)
		{
			if (user < 1) {
				return new MActionResult<List<UserLogin>>(StatusCode.NotValidID);
			}

			List<UserLogin> events = _ent.UserLogins.AsNoTracking()
										 .Where(x => x.UserID == user)
										 .OrderByDescending(x => x.Date)
										 .ToList();
			return new MActionResult<List<UserLogin>>(StatusCode.OK, events);
		}

		/// <summary>
		/// Creates the UserLogin
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="project">The project.</param>
		/// <param name="status">The status.</param>
		/// <param name="msg">The message.</param>
		/// <param name="ip">The IP adress.</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<UserLogin> Create(int user, Projects project, LoginStatus status, string ip,
											   string msg = null)
		{
			if (user < 1) {
				return new MActionResult<UserLogin>(StatusCode.NotValidID);
			}
			UserLogin l = new UserLogin() {
				UserID = user,
				ProjectID = (int) project,
				Date = DateTime.Now,
				Status = status,
				IP = ip,
			};

			UserLogin l1 = _ent.UserLogins.Add(l);
			Save(null);
			return new MActionResult<UserLogin>(StatusCode.OK, l1);
		}

		/// <summary>
		/// Saves UserLogin
		/// </summary>
		/// <param name="content">The Web Event to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(UserLogin content, bool throwException = true)
		{
			try {
				if (content != null) {
					_ent.UserLogins.AddOrUpdate(content);
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