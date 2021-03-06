﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace SharedApi.Connectors.New
{
	public class TransactionConnector : ApiConnector<Transaction>
	{
		public TransactionConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		public async Task<List<Transaction>> GetZolikTransactions(int zolikId)
		{
			if (zolikId < 1) {
				return new List<Transaction>();
			}
			try {
				var a = await Request($"transaction/getbyzolik?zolikId={zolikId}").GetJsonAsync<List<Transaction>>();
				return a;
			} catch {
				return new List<Transaction>();
			}
		}

		public async Task<MActionResult<List<Transaction>>> GetUserTransactions(int take = 100)
		{
			if (take < 1) {
				return new MActionResult<List<Transaction>>(StatusCode.InvalidInput);
			}
			try {
				var a = await Request($"transaction/getbyuser?take={take}")
							.GetJsonAsync<MActionResult<List<Transaction>>>();
				return a;
			} catch (Exception ex) {
				return new MActionResult<List<Transaction>>(StatusCode.SeeException, ex);
			}
		}
	}
}