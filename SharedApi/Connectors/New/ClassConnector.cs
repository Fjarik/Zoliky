using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared.ApiModels;

namespace SharedApi.Connectors.New
{
	public class ClassConnector : ApiConnector<Class>
	{
		public ClassConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		public async Task<MActionResult<Class>> GetAsync(int id)
		{
			if (id < 1) {
				return new MActionResult<Class>(StatusCode.NotValidID);
			}

			try {
				var res = await Request($"class/get/{id}").GetJsonAsync<MActionResult<Class>>();
				return res;
			} catch (Exception ex) {
#if (DEBUG)
				throw;
#endif
				return new MActionResult<Class>(StatusCode.SeeException, ex);
			}
		}

		public async Task<List<Class>> GetAllAsync()
		{
			try {
				var res = await Request("class/getall").GetJsonAsync<List<Class>>();
				return res;
			} catch {
#if (DEBUG)
				throw;
#endif
				return new List<Class>();
			}
		}
	}
}