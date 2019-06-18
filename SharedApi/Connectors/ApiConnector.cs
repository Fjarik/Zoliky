using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SharedApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Errors;

namespace SharedApi.Connectors
{
	public class ApiConnector
	{

		[NotNull]
		internal readonly string Url;

		[NotNull]
		public string UrlApi => $"{Url}/api/";

		[NotNull]
		public Version Version => new Version(1, 0, 1);


		[NotNull]
		private protected string DefaultName => "user";
		[NotNull]
		private protected string DefaultPassword => "It9ac8kw";


		[CanBeNull]
		internal string LoginToken => GetToken(DefaultName, DefaultPassword);

		[NotNull]
		private readonly FlurlClient _cli;

		[NotNull]
		private TimeSpan DefaultTimeout => TimeSpan.FromSeconds(30);


		[CanBeNull]
		public string UsedToken { get; set; }




		public ApiConnector(ApiUrl url = ApiUrl.Zoliky)
		{
			switch (url) {
				case ApiUrl.Localhost:
					Url = "http://localhost:89";
					break;
				default:
				case ApiUrl.Zoliky:
					Url = "https://www.zoliky.eu";
					break;
			}

			_cli = new FlurlClient(UrlApi).EnableCookies().AllowAnyHttpStatus().WithTimeout(DefaultTimeout);
			_cli.Headers.Add("User-Agent", $"ZolikApiConnector/{Version} (https://www.zoliky.eu; Autor: Jiří Falta)");
		}
		internal ApiConnector([NotNull]string token, ApiUrl url = ApiUrl.Zoliky) : this(url)
		{
			if (string.IsNullOrWhiteSpace(token)) {
				throw new ExpiredTokenException();
			}
			this.UsedToken = token;
		}



		public async Task<bool> GetBoolAsync(string url)
		{
			string s;
			try {
				HttpResponseMessage response = Task.Run(() => Request(url).GetAsync()).Result;
				if (response?.IsSuccessStatusCode != true) {
					return false;
				}
				s = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
			} catch {
#if DEBUG
				throw;
#endif
				return false;
			}
			if (string.IsNullOrWhiteSpace(s) || !bool.TryParse(s, out bool res)) {
				return false;
			}
			return res;

		}
		public Version GetVersion(string url)
		{
			try {
				string res = Task.Run(() => Request(url).GetStringAsync()).Result;
				if (res == null) {
					return null;
				}
				Version v = JsonConvert.DeserializeObject<Version>(res, new VersionConverter());
				return v;
			} catch {
#if DEBUG
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
			bool api = await CheckConnectionAsync().ConfigureAwait(true);
			bool db = await CheckDbConnectionAsync().ConfigureAwait(true);

			return (api && db);
		}

		public bool CheckConnections()
		{
			try {
				return Task.Run(() => CheckConnectionsAsync()).Result;
			} catch {
				return false;
			}
		}


		[CanBeNull]
		public Version GetApiVersion()
		{
			return GetVersion("public/apiversion");
		}


		/// <summary>
		/// Zkontroluje verzi api s Webovou částí. Vráti True, pokud jsou verze stejné.
		/// </summary>
		[NotNull]
		public bool CheckApiVersion()
		{
			return GetApiVersion() == this.Version;
		}

		[CanBeNull]
		public Version GetWpfVersion()
		{
			try {
				return GetVersion($"{UrlApi}/public/wpfversion");
			} catch {
#if DEBUG
				throw;
#endif
				return null;
			}
		}



		[CanBeNull]
		public WebStatus CheckWebStatus()
		{
			try {
				WebStatus ws = Task.Run(() => Request("public/webstatus").GetJsonAsync<WebStatus>()).Result;
				return ws;
			} catch {
#if DEBUG
				throw;
#endif
				return null;
			}

		}


		[NotNull]
		public string GetToken([NotNull]string username, [NotNull]string pwd)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pwd)) {
				username = DefaultName;
				pwd = DefaultPassword;
			}

			var kvp = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("username", username),
				new KeyValuePair<string, string>("password", pwd),
				new KeyValuePair<string, string>("grant_type", "password"),
			};
			try {

				HttpResponseMessage resMsg = $"{Url}/token"
					.AllowAnyHttpStatus()
					.WithTimeout(DefaultTimeout)
					.PostAsync(new FormUrlEncodedContent(kvp)).Result;
				if (resMsg?.IsSuccessStatusCode != true) {
					return "";
				}

				string res = resMsg.Content.ReadAsStringAsync().Result;
				if (string.IsNullOrWhiteSpace(res)) {
					return "";
				}
				JObject stuff = JObject.Parse(res);
				if (!stuff.ContainsKey("access_token")) {
					return "";
				}
				return stuff["access_token"].ToString();
			} catch {
#if DEBUG
				throw;
#endif
				return "";
			}

			/*
			using (WebClient wclient = new WebClient()) {

				NameValueCollection values = new NameValueCollection
				{
					["username"] = username,
					["password"] = pwd,
					["grant_type"] = "password"
				};
				byte[] response = wclient.UploadValues($"{Url}/token", values);
				string responseString = Encoding.Default.GetString(response);

				var stuff = Newtonsoft.Json.Linq.JObject.Parse(responseString);// JsonConvert.DeserializeObject<ExpandoObject>(responseString, product);

				return stuff["access_token"].ToString();
			}
			*/
		}


		#region Requests

		internal IFlurlRequest Request()
		{
			return Request(null);
		}

		internal IFlurlRequest Request([CanBeNull]string url)
		{
			return Request(url, null);
		}

		internal IFlurlRequest Request([CanBeNull]string url, [CanBeNull]string token)
		{
			return Request(url, token, TimeSpan.Zero);
		}

		internal IFlurlRequest Request([CanBeNull]string url, [CanBeNull]string token, TimeSpan time)
		{
			if (url == null) {
				url = "";
			}

			if (time.TotalSeconds < 5) {
				time = DefaultTimeout;
			}

			if (string.IsNullOrWhiteSpace(token)) {
				return _cli.Request(url).WithTimeout(time);
			}
			return _cli.Request(url).WithTimeout(time).WithOAuthBearerToken(token);
		}

		#endregion


	}
}
