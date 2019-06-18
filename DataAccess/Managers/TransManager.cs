using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using DataAccess.Errors;
using DataAccess.Models;
using JetBrains.Annotations;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class TransManager : IManager<Transaction>
	{
		ZoliksEntities _ent { get; set; }
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="TransManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public TransManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Transaction by ID
		/// </summary>
		/// <param name="id">Transaction ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Transaction> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Transaction>(StatusCode.NotValidID);
			}
			Transaction t = _ent.Transactions.Find(id);
			if (t == null) {
				return new MActionResult<Transaction>(StatusCode.NotFound);
			}
			//t = SetUsernames(t);
			return new MActionResult<Transaction>(StatusCode.OK, t);
		}

		/// <summary>
		/// Selects Transactions where occurs User ID
		/// </summary>
		/// <param name="userID">User ID</param>
		/// <param name="isTester">If selected user is Tester returns all Zoliks</param>
		/// <param name="lastTranID">ID of last Transaction</param>
		/// <returns>MActionResult of "List of Transactions"</returns>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		[NotNull]
		public MActionResult<List<Transaction>> UserTransactions(int userID, bool isTester = false, int? lastTranID = null) //ID uživatele - odesílatel/příjemce
		{
			if (userID < 1 || (lastTranID != null && lastTranID < 1)) {
				return new MActionResult<List<Transaction>>(StatusCode.NotValidID);
			}
			List<Transaction> t = new List<Transaction>();

			var query = _ent.Transactions.Where(x => x.FromID == userID || x.ToID == userID);
			if (!isTester) {
				query = query.Where(x => x.Zolik.Type != ZolikType.Debug);
			}

			query = query.OrderByDescending(x => x.Date);

			if (lastTranID == null) {
				t.AddRange(query.ToList());
			} else {
				t.AddRange(query.Where(x => x.ID > lastTranID).ToList());
			}

			return new MActionResult<List<Transaction>>(StatusCode.OK, t);
		}


		/// <summary>
		/// Creates new Transaction
		/// </summary>
		/// <param name="fromID">Sender ID</param>
		/// <param name="toID">Recipient ID</param>
		/// <param name="zolikID">ID of Zolik</param>
		/// <param name="msg">Message for recipient</param>
		/// <returns></returns>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Transaction> Create(int fromID, int toID, int zolikID, string msg, TransactionAssignment type)
		{
			if (fromID < 1 || toID < 1 || zolikID < 1) {
				return new MActionResult<Transaction>(StatusCode.NotValidID);
			}
			Transaction t = new Transaction() {
				ToID = toID,
				FromID = fromID,
				ZolikID = zolikID,
				Date = DateTime.Now,
				Message = msg,
				Typ = type
			};
			Transaction T = _ent.Transactions.Add(t);
			Save(null);
			//T = SetUsernames(T);
			return new MActionResult<Transaction>(StatusCode.OK, T);
		}

		/// <summary>
		/// Saves Transaction
		/// </summary>
		/// <param name="t">The Transaction to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(Transaction t, bool throwException = true)
		{
			try {
				if (t != null) {
					_ent.Transactions.AddOrUpdate(t);
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
		/*
		[CanBeNull]
		public Transaction SetUsernames([NotNull]Transaction t)
		{
			if (t == null) {
				return null;
			}

			if (t.FromUser != null) {
				t.From = t.FromUser.FullName;
			}

			if (t.ToUser != null) {
				t.To = t.ToUser.FullName;
			}

			return t;
		}
		*/
	}
}
