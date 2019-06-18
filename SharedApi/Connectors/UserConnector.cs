using Flurl.Http;
using Newtonsoft.Json;
using SharedApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharedLibrary.Enums;
using SharedLibrary;
using SharedLibrary.Interfaces;
using User = SharedApi.Models.User;

namespace SharedApi.Connectors
{
	public sealed class UserConnector : ApiConnector, IConnector<User>
	{

		public UserConnector(ApiUrl url = ApiUrl.Zoliky) : base(url) { }

		public UserConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }


		public MActionResult<User> Login(Logins lg)
		{
			if (lg == null || string.IsNullOrWhiteSpace(lg.UName) || string.IsNullOrWhiteSpace(lg.Password)) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			try {
				HttpResponseMessage response = Task.Run(() => Request($"user/login", LoginToken).PostJsonAsync(lg)).Result;
				string s = response.Content.ReadAsStringAsync().Result;
				if (response.StatusCode == HttpStatusCode.OK) {
					var mAu = JsonConvert.DeserializeObject<MActionResult<User>>(s);
					if (mAu.IsSuccess) {
						User u = mAu.Content;
						string tkn = GetToken(lg.UName, lg.Password);
						u.Token = tkn;
					}
					return mAu;
				}
				return new MActionResult<User>(StatusCode.SeeException, new Exception(s));
			} catch (Exception ex) {
				return new MActionResult<User>(StatusCode.SeeException, ex);
			}
		}


		public MActionResult<User> Get(int id)
		{
			if (id < 1 || string.IsNullOrWhiteSpace(UsedToken)) {
				return new MActionResult<User>(SharedLibrary.Enums.StatusCode.InvalidInput);
			}
			try {
				return Task.Run(() => Request($"user/get/{id}", UsedToken).GetJsonAsync<MActionResult<User>>()).Result;
			} catch (Exception ex) {
				return new MActionResult<User>(SharedLibrary.Enums.StatusCode.SeeException, ex);
			}
		}

		[NotNull]
		public MActionResult<User> GetTesterAccount([NotNull]User current)
		{
			if (current == null) {
				return new MActionResult<User>(StatusCode.InvalidInput);
			}

			try {
				var mAu = Task.Run(() => Request($"user/getTester", UsedToken).PostJsonAsync(current).ReceiveJson<MActionResult<User>>()).Result;
				if (mAu.IsSuccess) {
					mAu.Content.Token = current.Token;
				}
				return mAu;
			} catch (Exception ex) {
				return new MActionResult<User>(StatusCode.SeeException, ex);
			}
		}

		/// <summary>
		/// Returns all students in database
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public MActionResult<List<User>> GetStudents()
		{
			try {
				MActionResult<List<User>> result;
				string json = Task.Run(() => Request($"user/getstudents", UsedToken).GetStringAsync()).Result;
				try {
					result = JsonConvert.DeserializeObject<MActionResult<List<User>>>(json);
				} catch {
					List<User> users = JsonConvert.DeserializeObject<List<User>>(json);
					result = new MActionResult<List<User>>(StatusCode.OK, users);
				}
				return result;
			} catch (Exception ex) {
				return new MActionResult<List<User>>(StatusCode.SeeException, ex);
			}
		}

		[NotNull]
		public MActionResult<WebEvent> CreateEvent(Projects project, int? from, int to, int type, string msg)
		{
			try {
				WebEvent w = new WebEvent()
				{
					FromProjectID = (int)project,
					FromID = from,
					ToID = to,
					Type = type,
					Date = DateTime.Now,
					Enabled = true,
					Message = msg
				};
				string json = Task.Run(() => Request($"user/createevent", UsedToken).PostJsonAsync(w).ReceiveString()).Result;
				MActionResult<WebEvent> e = JsonConvert.DeserializeObject<MActionResult<WebEvent>>(json);
				return e;
			} catch (Exception ex) {
				return new MActionResult<WebEvent>(StatusCode.SeeException, ex);
			}
		}


	}
}
