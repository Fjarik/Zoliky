using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
using System.Xml;
using DataAccess.Models;
using Flurl.Http;
using Flurl.Util;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess
{
	public class WikiConnector
	{

		[NotNull]
		public string UrlApi => "http://wiki.skyzio.cz/api.php";

		[NotNull]
		public bool Logged { get; private set; }

		[NotNull]
		private readonly FlurlClient cli;

		public WikiConnector()
		{
			//_client = new WebClient();

			cli = new FlurlClient(UrlApi).EnableCookies().AllowAnyHttpStatus().WithTimeout(TimeSpan.FromSeconds(30));
			cli.Headers.Add("Api-User-Agent", "ZolikBot/1.0 (https://www.zoliky.eu; Autor: Jiří Falta)");
			Logged = Login();
		}

		[NotNull]
		public string GetToken([NotNull]string type)
		{
			if (string.IsNullOrWhiteSpace(type)) {
				return "";
			}

			var kvp = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("action" , "query"),
				new KeyValuePair<string, string>("meta" , "tokens"),
				new KeyValuePair<string, string>("type" , type),
				new KeyValuePair<string, string>("format" , "xml"),
			};

			string response = PostValuesAsync(kvp);
			if (string.IsNullOrWhiteSpace(response)) {
				return "";
			}

			XmlDocument doc = new XmlDocument();
			try {
				doc.LoadXml(response);
			} catch {
				return "";
			}
			var att = doc.SelectSingleNode($"api/query/tokens")?.Attributes?[$"{type}token"];
			string token = att?.Value;
			if (string.IsNullOrWhiteSpace(token)) {
				return "";
			}

			return token;
		}

		[NotNull]
		public MActionResult<User> CreateUser([NotNull]string username, [NotNull]string pwd, [NotNull]string email, [NotNull]string name)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pwd) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}


			string action = "createaccount";

			string token = GetToken(action);

			if (string.IsNullOrWhiteSpace(token)) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			NameValueCollection values = new NameValueCollection()
			{
				["action"] = action,
				["createreturnurl"] = "http://example.com/",
				["createtoken"] = token,
				["username"] = username,
				["password"] = pwd,
				["retype"] = pwd,
				["email"] = email,
				["realname"] = name,
				["format"] = "xml"
			};

			var kvp = new List<KeyValuePair<string, string>>();
			foreach (string value in values) {
				kvp.Add(new KeyValuePair<string, string>(value, values[value]));
			}


			string response = PostValuesAsync(kvp);
			if (string.IsNullOrWhiteSpace(response)) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			XmlDocument doc = new XmlDocument();
			try {
				doc.LoadXml(response);
			} catch (Exception ex) {
				return new MActionResult<User>(StatusCode.SeeException, ex);
			}

			var att = doc.SelectSingleNode($"api/{action}")?.Attributes;
			if (att?.Count < 1) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			string status = att?["status"]?.Value;
			if (string.IsNullOrWhiteSpace(status)) {
				return new MActionResult<User>(StatusCode.NotFound);
			}

			if (status.Equals("failed", StringComparison.CurrentCultureIgnoreCase)) {
				return new MActionResult<User>(StatusCode.AlreadyExists);
			}

			return new MActionResult<User>(StatusCode.OK);
		}

		public bool Login([CanBeNull]string token = null)
		{
			var kvp = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("action" , "login"),
				new KeyValuePair<string, string>("lgname" , "Admin@Zoliky"),
				new KeyValuePair<string, string>("lgpassword" , "tv7ojc7jn307t7l8e1oednlh4m74gbmo"),
				new KeyValuePair<string, string>("format" , "xml"),
			};
			if (!string.IsNullOrWhiteSpace(token)) {
				kvp.Add(new KeyValuePair<string, string>("lgtoken", token));
			}

			string response = PostValuesAsync(kvp);

			if (string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(response)) {
				XmlDocument doc = new XmlDocument();
				try {
					doc.LoadXml(response);
				} catch {
					return false;
				}

				var att = doc.SelectSingleNode($"api/login")?.Attributes?[$"token"];
				string tkn = att?.Value;
				if (string.IsNullOrWhiteSpace(tkn)) {
					return false;
				}
				return Login(tkn);
			}

			return true;
		}


		[CanBeNull]
		internal string PostValuesAsync([NotNull]List<KeyValuePair<string, string>> kvp)
		{
			if (kvp?.Count < 1) {
				return "";
			}
			return cli.Request().PostAsync(new FormUrlEncodedContent(kvp)).ReceiveString().Result;
		}


	}
}
