using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace SharedApi.Connectors.New
{
	public class UserConnector : ApiConnector<User>
	{
		public UserConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		public async Task<MActionResult<User>> GetMe()
		{
			try {
				var res = await Request("user/me").GetJsonAsync<MActionResult<User>>();
				return res;
			} catch (Exception ex) {
#if (DEBUG)
				throw;
#endif
				return new MActionResult<User>(StatusCode.SeeException, ex);
			}
		}

		public async Task<List<UserLogin>> GetUserLogins(int take = 50)
		{
			if (take < 1) {
				return new List<UserLogin>();
			}
			try {
				var res = await Request($"user/logins?take={take}").GetJsonAsync<List<UserLogin>>();
				return res;
			} catch (Exception ex) {
#if (DEBUG)
				throw;
#endif
				return new List<UserLogin>();
			}
		}
	}
}