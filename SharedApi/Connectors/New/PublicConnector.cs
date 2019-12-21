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
	}
}