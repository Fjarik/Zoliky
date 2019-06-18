using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Flurl.Http;

namespace ZolikyWeb.Tools
{
	public class OfflineHelper
	{
		public bool IsDown { get; private set; }
		public string Url { get; private set; }

		public OfflineHelper(string url)
		{
			this.IsDown = false;
			this.Url = url;
		}

		public void Init()
		{
			try {
				if (string.IsNullOrWhiteSpace(Url)) {
					this.IsDown = true;
					return;
				}

#region SSL ignore

				HttpClientHandler httpClientHandler = new HttpClientHandler {
					ServerCertificateCustomValidationCallback = (message, cert, chain,
																 errors) => true
				};
				HttpClient httpClient = new HttpClient(httpClientHandler) {
					BaseAddress = new Uri(this.Url)
				};
				var cli = new FlurlClient(httpClient)
						  .EnableCookies().AllowAnyHttpStatus().WithTimeout(TimeSpan.FromSeconds(10));

#endregion

				var res = Task.Run(() => cli.Request("/home/isdown").GetStringAsync()).Result;
				if (string.IsNullOrWhiteSpace(res)) {
					IsDown = true;
					return;
				}
				IsDown = bool.TryParse(res, out bool b) && b;
			} catch {
#if (DEBUG)
				throw;
#endif
				IsDown = true;
			}
		}
	}
}