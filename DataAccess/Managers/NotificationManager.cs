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
using SharedLibrary.Shared;

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

		public override Task<bool> DeleteAsync(Notification entity)
		{
			throw new NotSupportedException("Use HideNotificationAsync instead");
		}

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

		public Task<IList<Notification>> GetUserNotificationsAsync(int userId,
																   Projects? project = null,
																   int? top = null)
		{
			return GetUserNotificationsAsync(userId, (int?) project, top);
		}

		private async Task<IList<Notification>> GetUserNotificationsAsync(int userId,
																		  int? projectId = null,
																		  int? top = null)
		{
			if (userId < 1 || (projectId != null && projectId < 1)) {
				return new List<Notification>();
			}

			var query = _ctx.Notifications
							.Where(Extensions.IsNotExpired())
							.Where(x => x.ToID == userId &&
										x.ProjectID == projectId &&
										!x.Removed)
							.OrderBy(x => x.Seen)
							.ThenByDescending(x => x.Created);

			if (top != null) {
				query = (IOrderedQueryable<Notification>) query.Take((int) top);
			}

			return await query.ToListAsync();
		}

#endregion

#region Seen

		public Task<bool> AnyUnseenNotificationAsync(int userId)
		{
			return _ctx.Notifications
					   .Where(Extensions.IsNotExpired())
					   .AnyAsync(x => x.ToID == userId &&
									  !x.Seen);
		}

		public async Task<bool> SeenNotificationAsync(int notificationId, int userId)
		{
			var exists = await ExistsAsync(notificationId, userId);
			if (!exists) {
				return false;
			}
			return await SeenNotificationAsync(notificationId);
		}

		private async Task<bool> SeenNotificationAsync(int notificationId)
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

		public async Task<bool> HideNotificationAsync(int notificationId, int userId)
		{
			var exists = await ExistsAsync(notificationId, userId);
			if (!exists) {
				return false;
			}
			return await this.HideNotificationAsync(notificationId);
		}

		private async Task<bool> HideNotificationAsync(int notificationId)
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

		public async Task<bool> SendNotificationToStudentsAsync(string title,
																string subtitle,
																NotificationSeverity severity =
																	NotificationSeverity.Normal,
																DateTime? expiration = null)
		{
			var uMgr = Context.Get<UserManager>();
			var ids = (uMgr.GetStudents()).Select(x => x.ID).OrderBy(x => x);
			var success = true;

			foreach (var id in ids) {
				var res = await this.CreateAsync(id, title, subtitle, severity, expiration: expiration);
				if (!res.IsSuccess) {
					success = false;
					break;
				}
			}
			return success;
		}

		public Task<MActionResult<Notification>> CreateAsync(int toId,
															 string title,
															 string subtitle,
															 object content,
															 NotificationSeverity severity =
																 NotificationSeverity.Normal,
															 DateTime? expiration = null,
															 Projects? projectId = null)
		{
			var conJson = JsonConvert.SerializeObject(content);
			return this.CreateBasicAsync(toId, title, subtitle, severity, conJson, expiration, (int?) projectId);
		}

		public Task<MActionResult<Notification>> CreateAsync(int toId,
															 string title,
															 string subtitle,
															 NotificationSeverity severity =
																 NotificationSeverity.Normal,
															 DateTime? expiration = null,
															 Projects? projectId = null)
		{
			return this.CreateBasicAsync(toId, title, subtitle, severity, null, expiration, (int?) projectId);
		}

		private async Task<MActionResult<Notification>> CreateBasicAsync(int toId,
																		 string title,
																		 string subtitle,
																		 NotificationSeverity severity =
																			 NotificationSeverity.Normal,
																		 string content = null,
																		 DateTime? expiration = null,
																		 int? projectId = null)
		{
			if (toId < 1) {
				return new MActionResult<Notification>(StatusCode.NotValidID);
			}
			if (Methods.AreNullOrWhiteSpace(title, subtitle)) {
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
				Severity = severity,
				Expiration = expiration,
				Created = DateTime.Now,
				Removed = false,
				Seen = false,
				Icon = null
			};
			return await base.CreateAsync(ent);
		}

#endregion

#region Exists

		public Task<bool> ExistsAsync(int notId, int userId)
		{
			return _ctx.Notifications.AnyAsync(x => x.ID == notId && x.ToID == userId);
		}

#endregion

#endregion

#endregion
	}
}