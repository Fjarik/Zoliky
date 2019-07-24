using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class NotificationManager : Manager<Notification>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public NotificationManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public NotificationManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static NotificationManager Create(IdentityFactoryOptions<NotificationManager> options,
												 IOwinContext context)
		{
			return new NotificationManager(context);
		}

#endregion

#region Own Methods

#region Get user notifications

		public Task<IList<Notification>> GetUserNotificationsAsync(int userId, Projects? project = null)
		{
			return GetUserNotificationsAsync(userId, (int?) project);
		}

		private async Task<IList<Notification>> GetUserNotificationsAsync(int userId, int? projectId = null)
		{
			if (userId < 1 || (projectId != null && projectId < 1)) {
				return new List<Notification>();
			}
			return await _ctx.Notifications
							 .Where(Extensions.IsNotExpired())
							 .Where(x => x.ToID == userId &&
										 x.ProjectID == projectId &&
										 !x.Removed)
							 .OrderBy(x => x.Seen)
							 .ToListAsync();
		}

#endregion

#region Seen

		public async Task<bool> SeenNotificationAsync(int notificationId)
		{
			if (notificationId < 1) {
				return false;
			}
			var not = await this.GetByIdAsync(notificationId);
			if (!not.IsSuccess) {
				return false;
			}
			return await this.SeenNotificationAsync(not.Content);
		}

		private async Task<bool> SeenNotificationAsync(Notification not)
		{
			if (not == null) {
				return false;
			}
			not.Seen = true;
			return (await this.SaveAsync(not)) == 1;
		}

#endregion

#region Hide (remove)

		public async Task<bool> HideNotificationAsync(int notificationId)
		{
			if (notificationId < 1) {
				return false;
			}
			var not = await this.GetByIdAsync(notificationId);
			if (!not.IsSuccess) {
				return false;
			}
			return await this.RemoveNotificationAsync(not.Content);
		}

		private async Task<bool> RemoveNotificationAsync(Notification not)
		{
			if (not == null) {
				return false;
			}
			not.Removed = true;
			return (await this.SaveAsync(not)) == 1;
		}

#endregion

#region Create

		public Task<MActionResult<Notification>> CreateAsync(int toId,
															 string title,
															 string subtitle = null,
															 object content = null,
															 DateTime? expiration = null,
															 Projects? projectId = null)
		{
			var conJson = JsonConvert.SerializeObject(content);
			return this.CreateAsync(toId, title, subtitle, conJson, expiration, projectId);
		}

		public Task<MActionResult<Notification>> CreateAsync(int toId,
															 string title,
															 string subtitle = null,
															 string content = null,
															 DateTime? expiration = null,
															 Projects? projectId = null)
		{
			return this.CreateAsync(toId, title, subtitle, content, expiration, (int?) projectId);
		}

		public async Task<MActionResult<Notification>> CreateAsync(int toId,
																   string title,
																   string subtitle = null,
																   string content = null,
																   DateTime? expiration = null,
																   int? projectId = null)
		{
			if (toId < 1) {
				return new MActionResult<Notification>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(title)) {
				return new MActionResult<Notification>(StatusCode.InvalidInput);
			}
			if (expiration != null && expiration < DateTime.Now) {
				return new MActionResult<Notification>(StatusCode.InvalidInput);
			}
			if (projectId != null && !Enum.IsDefined(typeof(Projects), projectId)) {
				return new MActionResult<Notification>(StatusCode.NotValidID);
			}

			var ent = new Notification() {
				ToID = toId,
				ProjectID = projectId,
				Title = title,
				Subtitle = subtitle,
				Content = content,
				Expiration = expiration,
				Created = DateTime.Now,
				Removed = false,
				Seen = false
			};
			return await base.CreateAsync(ent);
		}

#endregion

#endregion

#endregion
	}
}