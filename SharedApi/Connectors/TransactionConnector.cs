using Flurl.Http;
using SharedApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Enums;
using SharedLibrary;
using SharedLibrary.Interfaces;

namespace SharedApi.Connectors
{
	public sealed class TransactionConnector : ApiConnector, IConnector<Transaction>
	{

		private TransactionConnector() : base() { }

		public TransactionConnector(string token) : base(token) { }

		public MActionResult<Transaction> Get(int id)
		{
			if (id < 1) {
				return new MActionResult<Transaction>(StatusCode.NotValidID);
			}

			try {
				return Task.Run(() =>
						Request($"transaction/get/{id}", UsedToken)
						.GetJsonAsync<MActionResult<Transaction>>())
						.Result;
			} catch (Exception ex) {
				return new MActionResult<Transaction>(StatusCode.SeeException, ex);
			}
		}

		/// <summary>
		/// Returns all transaction that user has made whether he send or received Žolík.
		/// </summary>
		/// <param name="userID">Users ID (sender/receiver)</param>
		/// <returns></returns>
		public MActionResult<List<Transaction>> GetTransactions(int userID)
		{
			if (userID < 1 || string.IsNullOrWhiteSpace(UsedToken)) {
				return new MActionResult<List<Transaction>>(StatusCode.NotValidID);
			}

			try {
				return Task.Run(() =>
					Request($"transaction/getbyuser/{userID}", UsedToken)
					.GetJsonAsync<MActionResult<List<Transaction>>>())
					.Result;
			} catch (Exception ex) {
				return new MActionResult<List<Transaction>>(StatusCode.SeeException, ex);
			}
		}

		/// <summary>
		/// Vrátí všechny transakce, ve kterých se uživatel angažoval (odesílatel/přijemce) od zadaného ID transakce
		/// </summary>
		/// <param name="userID">ID Uživatele (odesílatel/přijemce)</param>
		/// <param name="lastTranID">ID poslední transakce</param>
		/// <returns></returns>
		public MActionResult<List<Transaction>> GetTransactions(int userID, int lastTranID)
		{
			if (userID < 1 || lastTranID < 1 || string.IsNullOrWhiteSpace(UsedToken)) {
				return new MActionResult<List<Transaction>>(StatusCode.NotValidID);
			}

			try {
				return Task.Run(() =>
						Request($"transaction/getbyuser?userID={userID}&lastTransID={lastTranID}", UsedToken)
							.GetJsonAsync<MActionResult<List<Transaction>>>())
					.Result;
			} catch (Exception ex) {
				return new MActionResult<List<Transaction>>(StatusCode.SeeException, ex);
			}
		}


	}
}
