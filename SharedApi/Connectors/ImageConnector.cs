using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace SharedApi.Connectors
{
	public sealed class ImageConnector : ApiConnector, IConnector<Image>
	{
		private ImageConnector() : base() { }

		public ImageConnector(string token) : base(token) { }

		public MActionResult<Image> Get(int id)
		{
			if (id < 1) {
				return new MActionResult<Image>(StatusCode.NotValidID);
			}

			try {
				return Task.Run(() =>
									Request($"image/get/{id}", UsedToken)
										.GetJsonAsync<MActionResult<Image>>())
						   .Result;
			} catch (Exception ex) {
				return new MActionResult<Image>(StatusCode.SeeException, ex);
			}
		}

		public int GetSize(int id)
		{
			if (id < 1) {
				return 0;
			}

			try {
				return Task.Run(() =>
									Request($"image/getsize/{id}", UsedToken)
										.GetJsonAsync<int>())
						   .Result;
			} catch (Exception ex) {
				return 0;
			}
		}
	}
}