using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace SharedApi.Connectors.New
{
	public class SubjectConnector : ApiConnector<Subject>
	{
		public SubjectConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		public async Task<MActionResult<Subject>> GetAsync(int id)
		{
			if (id < 1) {
				return new MActionResult<Subject>(StatusCode.NotValidID);
			}

			try {
				var res = await Request($"subject/get/{id}").GetJsonAsync<MActionResult<Subject>>();
				return res;
			} catch (Exception ex) {
#if (DEBUG)
				throw;
#endif
				return new MActionResult<Subject>(StatusCode.SeeException, ex);
			}
		}

		public async Task<List<Subject>> GetAllAsync()
		{
			try {
				var res = await Request("subject/getall").GetJsonAsync<List<Subject>>();
				return res;
			} catch {
#if (DEBUG)
				throw;
#endif
				return new List<Subject>();
			}
		}
	}
}
