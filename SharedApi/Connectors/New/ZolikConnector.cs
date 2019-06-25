using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace SharedApi.Connectors.New
{
	public class ZolikConnector : ApiConnector<Zolik>
	{
		public ZolikConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		public async Task<MActionResult<Transaction>> TransferAsync(ZolikPackage p)
		{
			if (p == null || !p.IsValid) {
				return new MActionResult<Transaction>(StatusCode.InvalidInput);
			}

			try {
				var a = await Request("zolik/transfer").PostJsonAsync(p)
													   .ReceiveJson<MActionResult<Transaction>>();
				return a;
			} catch (Exception ex) {
				return new MActionResult<Transaction>(StatusCode.SeeException, ex);
			}
		}

		public Task<MActionResult<Transaction>> CreateAndTransferAsync(int teacherId,
																	   int toId,
																	   ZolikType type,
																	   int subjectId,
																	   string title,
																	   bool allowSplit)
		{
			var p = new ZolikCPackage() {
				TeacherId = teacherId,
				ToId = toId,
				SubjectId = subjectId,
				Type = type,
				Title = title,
				AllowSplit = allowSplit
			};
			return CreateAndTransferAsync(p);
		}

		public async Task<MActionResult<Transaction>> CreateAndTransferAsync(ZolikCPackage p)
		{
			if (p == null || !p.IsValid) {
				return new MActionResult<Transaction>(StatusCode.InvalidInput);
			}

			try {
				var a = await Request("zolik/ctransfer").PostJsonAsync(p)
														.ReceiveJson<MActionResult<Transaction>>();
				return a;
			} catch (Exception ex) {
				return new MActionResult<Transaction>(StatusCode.SeeException, ex);
			}
		}

		public async Task<List<Zolik>> GetUserZoliksAsync(int userId,
														  bool isTester = false,
														  bool onlyEnabled = true)
		{
			if (userId < 1) {
				return new List<Zolik>();
			}
			var url = $"zolik/getuserzoliklist?userId={userId}&isTester={isTester}&onlyEnabled={onlyEnabled}";
			try {
				var a = await Request(url).GetJsonAsync<List<Zolik>>();
				return a;
			} catch (Exception ex) {
				return new List<Zolik>();
			}
		}

		public async Task<List<int>> GetZolikOwnerIdsAsync()
		{
			var url = $"zolik/getzolikownerids";
			try {
				var a = await Request(url).GetJsonAsync<List<int>>();
				return a;
			} catch (Exception ex) {
				return new List<int>();
			}
		}
	}
}