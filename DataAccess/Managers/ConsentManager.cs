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
	public class ConsentManager
	{
		ZoliksEntities _ent { get; set; }
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsentManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public ConsentManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsentManager"/> class.
		/// </summary>
		public ConsentManager() : this(new ZoliksEntities(), null) { }

		/// <summary>
		/// Selects Consent by ID
		/// </summary>
		/// <param name="userID">User ID</param>
		/// <param name="termID">Term ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Consent> Select(int userID, int termID)
		{
			if (userID < 1 || termID < 1) {
				return new MActionResult<Consent>(StatusCode.NotValidID);
			}
			Consent t = _ent.Consents.SingleOrDefault(x => x.UserID == userID && x.TermID == termID);
			if (t == null) {
				return new MActionResult<Consent>(StatusCode.NotFound);
			}
			return new MActionResult<Consent>(StatusCode.OK, t);
		}

		/// <summary>
		/// Selects Consent by ID
		/// </summary>
		/// <param name="userID">User ID</param>
		/// <param name="term">Term</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Consent> Select(int userID, Terms term)
		{
			return Select(userID, (int) term);
		}


		/// <summary>
		/// Determines whether [term is accepted] by [the specified user].
		/// </summary>
		/// <param name="userID">The user identifier.</param>
		/// <param name="term">The term.</param>
		/// <returns>
		///   <c>true</c> if [term is accepted] by user; otherwise, <c>false</c>.
		/// </returns>
		public bool IsTermAccepted(int userID, Terms term)
		{
			var mAc = Select(userID, (int)term);
			if (!mAc.IsSuccess) {
				return false;
			}
			Consent c = mAc.Content;
			return c.Accepted;
		}


		/// <summary>
		/// Creates new Consent.
		/// </summary>
		/// <param name="userID">The user ID.</param>
		/// <param name="termID">The term ID.</param>
		/// <param name="accepted">If term is[accepted] by user.</param>
		/// <returns></returns>
		public MActionResult<Consent> Create(int userID, int termID, bool accepted = true)
		{
			if (userID < 1 || termID < 1) {
				return new MActionResult<Consent>(StatusCode.NotValidID);
			}
			Consent c = new Consent() {
				UserID = userID,
				TermID = termID,
				When = DateTime.Now,
				Accepted = accepted
			};
			Consent c1 = _ent.Consents.Add(c);
			Save(null);
			return new MActionResult<Consent>(StatusCode.OK, c1);
		}

		/// <summary>
		/// Creates new Consent.
		/// </summary>
		/// <param name="userID">The user ID.</param>
		/// <param name="term">The term.</param>
		/// <param name="accepted">If term is[accepted] by user.</param>
		/// <returns></returns>
		public MActionResult<Consent> Create(int userID, Terms term, bool accepted = true)
		{
			return Create(userID, (int)term, accepted);
		}


		/// <summary>
		/// Saves Transaction
		/// </summary>
		/// <param name="c">The Transaction to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(Consent c, bool throwException = true)
		{
			try {
				if (c != null) {
					_ent.Consents.AddOrUpdate(c);
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
