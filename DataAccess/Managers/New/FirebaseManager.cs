using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using FCM.Net;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;

namespace DataAccess.Managers.New
{
	public sealed class FirebaseManager : IDisposable
	{
		/// 
		/// Fields
		///
		private readonly IOwinContext _context;

		private const string ServerKey =
			"AAAAw_H6Y_A:APA91bGn3PJe1LaSOjLIv_SqNdoWaNlFaxc2WcgdTGU2VnZOvZqvroBBRqDFN-AvI-OltZHuZYNpNvaPh-QonEz9ie2pd0OOl21JmMFUBu7MWrahokO-aXA_TLRjF61xqTRsXWjsPfhCAmN_Fv6E06dSdFdaIwWdtw";

		/// 
		/// Constructors
		/// 
		public FirebaseManager(IOwinContext context)
		{
			this._context = context;
		}

		// Methods

#region Methods

		/// 
		/// Own methods
		/// 

#region Static methods

		public static FirebaseManager Create(IdentityFactoryOptions<FirebaseManager> options,
											 IOwinContext context)
		{
			return new FirebaseManager(context);
		}

#endregion

#region Own Methods

		public async Task<bool> NewZolikAsync(int zolikId, int fromId, int toId)
		{
			if (zolikId < 1 || toId < 1 || fromId < 1) {
				return false;
			}
			var uMgr = _context.Get<ZolikManager>();
			var res = await uMgr.GetByIdAsync(zolikId);
			if (!res.IsSuccess) {
				return false;
			}
			return await this.NewZolikAsync(res.Content, fromId, toId);
		}

		public async Task<bool> NewZolikAsync(IZolik z, int fromId, int toId)
		{
			if (toId < 1 || fromId < 1) {
				return false;
			}
			var uMgr = _context.Get<UserManager>();
			string name = await uMgr.GetUserFullnameAsync(fromId);
			return await this.NewZolikAsync(z, name, toId);
		}

		private async Task<bool> NewZolikAsync(IZolik z, string fromName, int toId)
		{
			if (toId < 1) {
				return false;
			}
			var uMgr = _context.Get<UserSettingManager>();
			var res = await uMgr.GetAllByUserIdAsync(toId);
			if (!res.IsSuccess) {
				return false;
			}
			return await this.NewZolikAsync(z, fromName, res.Content);
		}

		private async Task<bool> NewZolikAsync(IZolik z, string fromName, ICollection<UserSetting> settings)
		{
			string key = SettingKeys.MobileToken;
			if (settings == null || settings.All(x => x.Key != key)) {
				return false;
			}
			return await this.NewZolikAsync(z.Type.GetDescription().ToLower(), z.Title, fromName, z, settings.GetStringValue(key));
		}

		private async Task<bool> NewZolikAsync(string type, string zTitle, string fromName, IZolik data, string token)
		{
			if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(fromName)) {
				return false;
			}
			string title = $"Nový {type}";
			string body = $"{zTitle}, od: {fromName}";
			var d = new {
				Title = data.Title,
				Type = data.Type
			};
			return await this.SendAsync(title, body, d, token);
		}

		private async Task<bool> SendAsync(string title, string body, object data, params string[] tokens)
		{
			if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body) || tokens.Length < 1) {
				return false;
			}

			using (var notSender = new Sender(ServerKey)) {
				var msg = new Message() {
					RegistrationIds = tokens.ToList(),
					Notification = new FCM.Net.Notification() {
						Title = title,
						Body = body,
						ClickAction = "FLUTTER_NOTIFICATION_CLICK"
					},
					Data = data,
					Priority = Priority.Normal,
					ContentAvailable = data != null
				};
				var result = await notSender.SendAsync(msg);
				if (result.StatusCode != HttpStatusCode.OK) {
					return false;
				}
			}
			return true;
		}

#endregion

#endregion

		public void Dispose() { }
	}
}