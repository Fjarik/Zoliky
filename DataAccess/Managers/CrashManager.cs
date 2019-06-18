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
	public class CrashManager : IManager<Crash>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="CrashManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public CrashManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Crash by ID.
		/// </summary>
		/// <param name="id">Crash ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.NotEnabled" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Crash> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Crash>(StatusCode.NotValidID);
			}

			Crash c = _ent.Crashes.Find(id);
			if (c == null) {
				return new MActionResult<Crash>(StatusCode.NotFound);
			}
			if (!c.Enabled) {
				return new MActionResult<Crash>(StatusCode.NotEnabled, c);
			}
			return new MActionResult<Crash>(StatusCode.OK, c);
		}

		/// <summary>
		/// Gets all Crashes.
		/// </summary>
		public MActionResult<List<Crash>> GetAll()
		{
			List<Crash> c = _ent.Crashes.Where(x => x.Enabled).OrderBy(x => x.Date).ToList();
			return new MActionResult<List<Crash>>(StatusCode.OK, c);
		}

		/// <summary>
		/// Gets all Crashes from specified user.
		/// </summary>
		/// <param name="userID">The user ID</param>
		/// <returns></returns>
		public MActionResult<List<Crash>> GetAllFromUser(int? userID)
		{
			List<Crash> c = _ent.Crashes.Where(x => x.Enabled && x.UserID == userID).OrderBy(x => x.Date).ToList();
			return new MActionResult<List<Crash>>(StatusCode.OK, c);
		}

		/// <summary>
		/// Creates new Crash report
		/// </summary>
		/// <param name="project">The project ID.</param>
		/// <param name="screenid">The screenshot ID.</param>
		/// <param name="fromUser">Who sent report.</param>
		/// <param name="appVersion">The application version.</param>
		/// <param name="build">The application build.</param>
		/// <param name="lastWin">The last win.</param>
		/// <param name="os">The os.</param>
		/// <param name="isOS64Bit">The is o S64 bit.</param>
		/// <param name="isApp64Bit">The is app64 bit.</param>
		/// <param name="when">The when.</param>
		/// <param name="ex">The ex.</param>
		/// <param name="msg">The MSG.</param>
		/// <param name="email">The email.</param>
		/// <param name="log">The log.</param>
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Crash> Create(Projects project, int? screenid, int? fromUser, string appVersion, DateTime? build, string lastWin, string os, bool? isOS64Bit, bool? isApp64Bit, DateTime when, Exception ex, string msg, string email, string log)
		{
			if (string.IsNullOrWhiteSpace(appVersion) || string.IsNullOrWhiteSpace(os) || ex == null || string.IsNullOrWhiteSpace(log)) {
				return new MActionResult<Crash>(StatusCode.InvalidInput);
			}

			Crash c = new Crash()
			{
				ProjectID = (int)project,
				ScreenshotID = screenid,
				UserID = fromUser,
				Status = CrashStatus.Created,
				AppVersion = appVersion,
				Build = build,
				LastWindowName = lastWin,
				OS = os,
				Is64BitOS = isOS64Bit,
				Is64BitApp = isApp64Bit,
				Date = when,
				Exception = ex.ToString(),
				Message = msg,
				Email = email,
				Log = log,
				Enabled = true
			};
			Crash c1 = _ent.Crashes.Add(c);
			Save(null);
			return new MActionResult<Crash>(StatusCode.OK, c1);
		}

		/// <summary>
		/// Saves Crash
		/// </summary>
		/// <param name="p">The CrashPackage.</param>
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Crash> Create(CrashPackage p)
		{
			return this.Create(p.Project, null, p.FromID, p.AppVersion, p.Build, p.LastWindows, p.OS, p.Is64BitOS,
				p.Is64BitApp, p.When, p.Exception, p.Message, p.Email, p.Log);
		}

		/// <summary>
		/// Saves Crash
		/// </summary>
		/// <param name="c">The Crash to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(Crash c, bool throwException = true)
		{
			try {
				if (c != null) {
					_ent.Crashes.AddOrUpdate(c);
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
