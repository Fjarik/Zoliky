using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace DataAccess
{
	public class GoogleConnector
	{
		private const string SiteKey = "6LeQVGkUAAAAAMULFUud0vzwAU2Qfj4O2kK38W6L";
		private const string SecretKey = "6LeQVGkUAAAAAFTYPYOyAsslKU7vfZQ35TS3CatF";


		[NotNull]
		public static RecaptchaResult Verify([NotNull]string response, [NotNull]string action, [CanBeNull] string ip = null)
		{
			if (string.IsNullOrWhiteSpace(response) || string.IsNullOrWhiteSpace(action)) {
				return new RecaptchaResult() { Success = false };
			}

			var kvp = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("secret" , SecretKey),
				new KeyValuePair<string, string>("response" , response),
			};
			if (!string.IsNullOrWhiteSpace(ip)) {
				kvp.Add(new KeyValuePair<string, string>("remoteip", ip));
			}
			string s = "https://www.google.com/recaptcha/api/siteverify".AllowAnyHttpStatus().WithTimeout(TimeSpan.FromSeconds(5))
				.PostAsync(new FormUrlEncodedContent(kvp)).ReceiveString().Result;
			RecaptchaResult res = JsonConvert.DeserializeObject<RecaptchaResult>(s);
			if (!res.Success) {
				return res;
			}

			if (string.IsNullOrWhiteSpace(res.Action) || !res.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase)) {
				res.Success = false;
			}

			return res;
		}

	}

	public class RecaptchaResult
	{
		[NotNull]
		[JsonProperty("success")]
		public bool Success { get; set; } = false;

		[CanBeNull]
		[JsonProperty("score")]
		public double? Score { get; set; }

		[CanBeNull]
		[JsonProperty("action")]
		public string Action { get; set; }

		[CanBeNull]
		[JsonProperty("challenge_ts")]
		public DateTime? TimeStamp { get; set; }

		[CanBeNull]
		[JsonProperty("hostname")]
		public string Hostname { get; set; }

		[CanBeNull]
		[JsonProperty("error-codes")]
		public string[] ErrorCodes { get; set; }
	}
}
