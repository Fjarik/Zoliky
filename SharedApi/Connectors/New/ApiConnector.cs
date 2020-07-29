using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SharedLibrary;
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

		private string CurrentProject => ((int) Projects.UWP).ToString();

		// protected Task<string> LoginTokenAsync => GetTokenAsync(DefaultName, DefaultPassword);

#endregion

#region Get, set

		public string UsedToken { get; protected set; }

#endregion

#region Constructors

		protected ApiConnector(ApiUrl url = ApiUrl.Zoliky)
		{
			_url = "https://api.zoliky.eu/";
			if (url == ApiUrl.Localhost) {
				_url = "http://localhost:93/";
			}
			_cli = new FlurlClient(_url).EnableCookies().AllowAnyHttpStatus().WithTimeout(DefaultTimeout);
			_cli.Headers.Add("User-Agent", $"ZolikApiConnector/{Version} (http://www.zoliky.eu; Autor: Jiří Falta)");
			_cli.Headers.Add("api-version", Version.ToString());
			if (!_cli.Headers.ContainsKey("projectId")) {
				_cli.Headers.Add("projectId", CurrentProject);
			}
		}

		protected ApiConnector(string token, ApiUrl url = ApiUrl.Zoliky) : this(url)
		{
			if (!string.IsNullOrWhiteSpace(token)) {
				this.UsedToken = token;
			}
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

		public async Task<MActionResult<string>> GetTokenAsync(string username, string pwd)
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
				var resMsg = await $"{_url}/token".AllowAnyHttpStatus()
												  .WithTimeout(DefaultTimeout)
												  .WithHeader("projectId", CurrentProject)
												  .PostAsync(new FormUrlEncodedContent(kvp));

				if (resMsg?.StatusCode != HttpStatusCode.BadRequest && resMsg?.IsSuccessStatusCode != true) {
					return new MActionResult<string>(StatusCode.InternalError);
				}

				string res = await resMsg?.Content.ReadAsStringAsync();
				if (string.IsNullOrWhiteSpace(res)) {
					return new MActionResult<string>(StatusCode.NotFound);
				}

				var stuff = JObject.Parse(res);
				if (resMsg.StatusCode == HttpStatusCode.BadRequest &&
					stuff.ContainsKey("error") &&
					stuff["error"].ToString() is string error) {
					if (error == "invalid_grant") {
						return new MActionResult<string>(StatusCode.InternalError, error);
					}
					if (int.TryParse(error, out int code)) {
						return new MActionResult<string>((StatusCode) code);
					}
					return new MActionResult<string>(StatusCode.InternalError);
				}

				if (!stuff.ContainsKey("access_token")) {
					return new MActionResult<string>(StatusCode.NotFound);
				}
				var token = stuff["access_token"].ToString();
				return new MActionResult<string>(StatusCode.OK, token);
			} catch (Exception ex) {
#if DEBUG
				throw;
#endif
				return new MActionResult<string>(StatusCode.SeeException, ex);
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