using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using DataAccess.Errors;
using DataAccess.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class NotificationManager : IManager<Notification>
	{
		// TODO: Opravit
		private ZoliksEntities _ent;
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public NotificationManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Notificaiton by ID.
		/// </summary>
		/// <param name="id">Notification ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.NotEnabled" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Notification> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Notification>(StatusCode.NotValidID);
			}

			Notification n = _ent.Notifications.Find(id);
			if (n == null) {
				return new MActionResult<Notification>(StatusCode.NotFound);
			}

			if (!n.Visible) {
				return new MActionResult<Notification>(StatusCode.NotEnabled, n);
			}
			return new MActionResult<Notification>(StatusCode.OK, n);
		}

		/// <summary>
		/// Gets all Notification of user.
		/// </summary>
		/// <param name="userID">User ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<List<Notification>> GetAll(int userID)
		{
			if (userID < 1) {
				return new MActionResult<List<Notification>>(StatusCode.NotValidID);
			}

			//List<Notification> all = _ent.Notifications.AsNoTracking().Where(x => x.Visible).ToList();
			List<Notification> all = _ent.GetAllNotifications().ToList();
			//	List<Notification> n = _ent.Notifications.AsNoTracking().Where(x => x.ToID == userID && x.Visible && !x.IsExpired).ToList();

			List<Notification> n = all.Where(x => x.ToID == userID).ToList();

			n.AddRange(all.Where(x => x.Type == NotificationType.Global));
			n.AddRange(all.Where(x => x.Type == NotificationType.Project));

			var read = _ent.Users.FirstOrDefault(x => x.ID == userID)?.ReadNotifications;

			if (read?.Count > 0) {
				foreach (Notification not in read) {
					n.RemoveAll(x => x.ID == not.ID);
				}
			}

			n = n.OrderByDescending(x => x.Created).ToList();
			return new MActionResult<List<Notification>>(StatusCode.OK, n);
		}

		/// <summary>
		/// Creates new Notification
		/// </summary>
		/// <param name="project">Project</param>
		/// <param name="fromID">From user</param>
		/// <param name="toID">To user</param>
		/// <param name="urgent">Is notification urgent?</param>
		/// <param name="content">The content of Notification</param>
		/// <param name="expiration">The expiration date of Notification</param>
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		private MActionResult<Notification> Create(Projects? project, int? fromID, int? toID, string title,
												   string content,
												   DateTime? expiration)
		{
			if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(title)) {
				return new MActionResult<Notification>(StatusCode.InvalidInput);
			}

			if (project == null && fromID == null && toID == null) {
				return new MActionResult<Notification>(StatusCode.InvalidInput);
			}

			if ((fromID != null && fromID < 1) || (toID != null && toID < 1)) {
				return new MActionResult<Notification>(StatusCode.InvalidInput);
			}

			Notification n = new Notification() {
				ToID = toID,
				FromID = fromID,
				ProjectID = (int?) project,
				Created = DateTime.Now,
				Visible = true,
				Title = title,
				Content = content,
				Expiration = expiration
			};
			Notification n1 = _ent.Notifications.Add(n);
			Save(null);
			return new MActionResult<Notification>(StatusCode.OK, n1);
		}

		public MActionResult<Notification> CreateUserNotification(int toID, string title, string content,
																  DateTime? expiration, int? fromID = null)
		{
			return this.Create(null, fromID, toID, title, content, expiration);
		}

		public MActionResult<Notification> CreateGlobalNotification(string title, string content,
																	DateTime? expiration, int? fromID = null)
		{
			return this.Create(null, fromID, null, title, content, expiration);
		}

		public MActionResult<Notification> CreateProjectNotification(Projects? project, string title, string content,
																	 DateTime? expiration, int? fromID = null)
		{
			return this.Create(project, fromID, null, title, content, expiration);
		}

		/// <summary>
		/// Saves Notification
		/// </summary>
		/// <param name="n">The Notification to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(Notification n, bool throwException = true)
		{
			try {
				if (n != null) {
					_ent.Notifications.AddOrUpdate(n);
				}
				int changes = _ent.SaveChanges();
				return changes;
			} catch (DbEntityValidationException ex) {
				if (throwException) {
					throw new DbEntityValidationException(ex.GetExceptionMessage(), ex.EntityValidationErrors);
				}
				return 0;
			}
		}
	}
}