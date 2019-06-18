using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SharedLibrary.Enums;
using SharedLibrary.Errors;
using SharedLibrary.Interfaces;

namespace SharedApi.Connectors.New
{
	public abstract class ApiConnector
	{
#region "Fields"

		protected readonly string _url;

		protected readonly FlurlClient _cli;

#endregion

#region Get only

		public virtual string Url => _url;

		public static Version Version => new Version(2, 0);
		private string DefaultName => "user";
		private string DefaultPassword => "It9ac8kw";
		private TimeSpan DefaultTimeout => TimeSpan.FromSeconds(30);

		protected Task<string> LoginTokenAsync => GetTokenAsync(DefaultName, DefaultPassword);

#endregion

#region Get, set

		protected string UsedToken { get; set; }

#endregion

#region Constructors

		protected ApiConnector(ApiUrl url = ApiUrl.Zoliky)
		{
			_url = "https://api.zoliky.eu/";
			if (url == ApiUrl.Localhost) {
				_url = "http://localhost:93/";
			}
			_cli = new FlurlClient(_url).EnableCookies().AllowAnyHttpStatus().WithTimeout(DefaultTimeout);
			_cli.Headers.Add("User-Agent", $"ZolikApiConnector/{Version} (https://www.zoliky.eu; Autor: Jiří Falta)");
			_cli.Headers.Add("api-version", Version.ToString());
		}

		protected ApiConnector(string token, ApiUrl url = ApiUrl.Zoliky) : this(url)
		{
			if (string.IsNullOrWhiteSpace(token)) {
				throw new ArgumentNullException(nameof(token));
			}
			this.UsedToken = token;
		}

#endregion

#region Get basic objects

		public async Task<bool> GetBoolAsync(string url)
		{
			try {
				string res = await PublicRequest(url).GetStringAsync();
				if (string.IsNullOrWhiteSpace(res)) {
					return false;
				}
				return bool.TryParse(res, out bool b) && b;
			} catch {
#if DEBUG
				throw;
#endif
				return false;
			}
		}

		public async Task<Version> GetVersionAsync(string url)
		{
			try {
				string res = await PublicRequest(url).GetStringAsync();
				if (string.IsNullOrWhiteSpace(res)) {
					return null;
				}
				var v = JsonConvert.DeserializeObject<Version>(res, new VersionConverter());
				return v;
			} catch {
#if DEBUG
				throw;
#endif
				return null;
			}
		}

#endregion

		public async Task<string> GetTokenAsync(string username, string pwd)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pwd)) {
				username = DefaultName;
				pwd = DefaultPassword;
			}

			var kvp = new List<KeyValuePair<string, string>>() {
				new KeyValuePair<string, string>("username", username),
				new KeyValuePair<string, string>("password", pwd),
				new KeyValuePair<string, string>("grant_type", "password"),
			};
			try {
				HttpResponseMessage resMsg = await $"{_url}/token"
												   .AllowAnyHttpStatus()
												   .WithTimeout(DefaultTimeout)
												   .PostAsync(new FormUrlEncodedContent(kvp));
				if (resMsg?.IsSuccessStatusCode != true) {
					return "";
				}

				string res = await resMsg.Content.ReadAsStringAsync();
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
		}

#region Requests

		protected virtual IFlurlRequest PublicRequest(string url)
		{
			return PublicRequest(url, TimeSpan.Zero);
		}

		protected virtual IFlurlRequest PublicRequest(string url, TimeSpan time)
		{
			return Request(url, null, time);
		}

		protected virtual IFlurlRequest Request(string url)
		{
			return Request(url, this.UsedToken);
		}

		protected virtual IFlurlRequest Request(string url, string token)
		{
			return Request(url, token, TimeSpan.Zero);
		}

		protected virtual IFlurlRequest Request(string url, string token, TimeSpan time)
		{
			if (url == null) {
				url = "";
			}

			if (time.TotalSeconds < 5) {
				time = DefaultTimeout;
			}

			var r = _cli.Request(url).WithTimeout(time);
			if (string.IsNullOrWhiteSpace(token)) {
				return r;
			}
			return r.WithOAuthBearerToken(token);
		}

#endregion
	}

	public abstract class ApiConnector<T> : ApiConnector where T : IDbEntity
	{
		protected ApiConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }
	}
}