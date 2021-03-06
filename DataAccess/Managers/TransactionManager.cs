﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class TransactionManager : Manager<Transaction>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public TransactionManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public TransactionManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

#region Overrides

		public override Task<bool> DeleteAsync(Transaction entity)
		{
			throw new NotSupportedException("Přímé nazání transakcí z databáze není možné.");
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static TransactionManager Create(IdentityFactoryOptions<TransactionManager> options,
												IOwinContext context)
		{
			return new TransactionManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<List<Transaction>>> UserTransactionsAsync(
			int userID, bool isTester = false, int? lastTranID = null,
			int take = 100) //ID uživatele - odesílatel/příjemce
		{
			if (userID < 1 || (lastTranID != null && lastTranID < 1)) {
				return new MActionResult<List<Transaction>>(StatusCode.NotValidID);
			}

			var query = _ctx.Transactions.Where(x => x.FromID == userID || x.ToID == userID);
			if (!isTester) {
				query = query.Where(x => !x.Zolik.Type.IsTestType);
			}

			query = query.OrderByDescending(x => x.Date);

			if (lastTranID != null) {
				query = query.Where(x => x.ID > lastTranID);
			}
			var res = await query.Take(take).ToListAsync();

			return new MActionResult<List<Transaction>>(StatusCode.OK, res);
		}

		public async Task<List<Transaction>> ZolikTransactionsAsync(int zolikid) //ID uživatele - odesílatel/příjemce
		{
			if (zolikid < 1) {
				return new List<Transaction>();
			}
			var res = await _ctx.Transactions
								.Where(x => x.ZolikID == zolikid)
								.OrderByDescending(x => x.Date)
								.ToListAsync();

			return res;
		}

		public async Task<MActionResult<Transaction>> CreateAsync(int fromID,
																  int toID,
																  int zolikID,
																  string msg,
																  TransactionAssignment type)
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
			return await this.CreateAsync(t);
		}

		public async Task<int> CountUserTransactionsAsync(int userId, bool incTester = false)
		{
			if (userId < 1) {
				return 0;
			}

			var query = _ctx.Transactions.Where(x => x.FromID == userId || x.ToID == userId);
			if (!incTester) {
				query = query.Where(x => !x.Zolik.Type.IsTestType);
			}

			return await query.CountAsync();
		}

#endregion

#endregion
	}
}