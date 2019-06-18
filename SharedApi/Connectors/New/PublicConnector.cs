using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace SharedApi.Connectors.New
{
	public class PublicConnector : ApiConnector
	{
		public override string Url => $"{_url}/public";

		public PublicConnector(ApiUrl url = ApiUrl.Zoliky) : base(url) { }

		public async Task<Version> GetWpfVersionAsync()
		{
			try {
				return await base.GetVersionAsync($"{Url}/wpfversion");
			} catch {
#if (DEBUG)
				throw;
#endif
				return null;
			}
		}

		public Task<bool> CheckConnectionAsync()
		{
			return GetBoolAsync("public/connection");
		}

		public Task<bool> CheckDbConnectionAsync()
		{
			return GetBoolAsync("public/dbconnection");
		}

		public async Task<bool> CheckConnectionsAsync()
		{
			bool api = await CheckConnectionAsync();
			bool db = await CheckDbConnectionAsync();

			return (api && db);
		}

		public async Task<WebStatus> CheckStatusAsync(Projects project)
		{
			try {
				var param = $"projectId={(int) project}";
				var ws = await PublicRequest($"public/status?{param}").GetJsonAsync<WebStatus>();
				return ws;
			} catch {
#if DEBUG
				throw;
#endif
				return null;
			}
		}

		public async Task<MActionResult<User>> LoginAsync(Logins lg)
		{
			if (lg == null || string.IsNullOrWhiteSpace(lg.UName) || string.IsNullOrWhiteSpace(lg.Password)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			try {
				var ltoken = await LoginTokenAsync;
				var response = await Request($"user/login", ltoken).PostJsonAsync(lg);
				string s = await response.Content.ReadAsStringAsync();
				if (response.StatusCode != HttpStatusCode.OK) {
					return new MActionResult<User>(StatusCode.SeeException, new Exception(s));
				}
				var mAu = JsonConvert.DeserializeObject<MActionResult<User>>(s);
				if (mAu.IsSuccess) {
					User u = mAu.Content;
					string tkn = await GetTokenAsync(lg.UName, lg.Password);
					u.Token = tkn;
				}
				return mAu;
			} catch (Exception ex) {
				return new MActionResult<User>(StatusCode.SeeException, ex);
			}
		}
	}
}