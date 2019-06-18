using Flurl.Http;
using SharedApi.Models;
using System;
using System.Collections.Generic;
using SharedLibrary;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace SharedApi.Connectors
{
	public sealed class ZolikConnector : ApiConnector, IConnector<Zolik>
	{
		private ZolikConnector(ApiUrl url = ApiUrl.Zoliky) : base(url) { }

		public ZolikConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		public MActionResult<Zolik> Get(int id)
		{
			if (id < 1 || string.IsNullOrWhiteSpace(UsedToken)) {
				return new MActionResult<Zolik>(StatusCode.InvalidInput);
			}

			try {
				return Task.Run(() => Request($"zolik/get/{id}", UsedToken).GetJsonAsync<MActionResult<Zolik>>()).Result;
			} catch (Exception ex) {
				return new MActionResult<Zolik>(StatusCode.SeeException, ex);
			}

		}

		public MActionResult<List<Zolik>> GetZoliks(int ownerId)
		{
			if (ownerId < 1 || string.IsNullOrWhiteSpace(UsedToken)) {
				return new MActionResult<List<Zolik>>(StatusCode.InvalidInput);
			}

			try {
				return Task.Run(() => Request($"zolik/getuserzoliks/{ownerId}", UsedToken).GetJsonAsync<MActionResult<List<Zolik>>>()).Result;
			} catch (Exception ex) {
				return new MActionResult<List<Zolik>>(StatusCode.SeeException, ex);
			}

		}

		public MActionResult<List<Zolik>> GetZoliks([NotNull]User u)
		{
			if (u == null || string.IsNullOrWhiteSpace(UsedToken) || u.ID < 1) {
				return new MActionResult<List<Zolik>>(StatusCode.InvalidInput);
			}

			try {
				if (u.IsTesterType) {
					return Task.Run(() =>
						Request("zolik/getuserzoliks/", UsedToken).PostJsonAsync(u)
							.ReceiveJson<MActionResult<List<Zolik>>>()).Result;
				} else {
					return Task.Run(() => Request($"zolik/getuserzoliks/{u.ID}", UsedToken).GetJsonAsync<MActionResult<List<Zolik>>>()).Result;
				}
			} catch (Exception ex) {
				return new MActionResult<List<Zolik>>(StatusCode.SeeException, ex);
			}

		}

		public MActionResult<User> Transfer(ZolikPackage p) // TODO: To MA<Transaction>
		{
			if (p == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			try {
				var a = Task.Run(() =>
						Request($"user/transfer", UsedToken).PostJsonAsync(p)
							.ReceiveJson<MActionResult<User>>())
					.Result;
				return a;
			} catch (Exception ex) {
				return new MActionResult<User>(StatusCode.SeeException, ex);
			}
		}





	}
}
